using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ImuSensor : MonoBehaviour
{
    // ROS에서 사용될 데이터
    [Header("Imu Sensor Parameter")]
    public Vector3 linearAcceleration;
    public Vector3 angularVelocity;
    public Quaternion orientation;

    // 유니티에서 사용될 데이터
    private Vector3 unityAngularVelocity;
    private Vector3 unityLinearAcceleration;

    // 실제 데이터 추출용 변수
    private Rigidbody playerRigidBody;
    private Vector3 previousVelocity;
    private PlayerState sensorstate;

    // 노이즈 변수
    private float gaussianNoiseStdDev = 0.05f; // 가우시안 노이즈 표준편차
    private float driftRate = 0.001f; // 바이어스 드리프트 속도

    // 노이즈 계산 변수
    private Vector3 biasDrift;

    void Awake()
    {
        playerRigidBody = GetComponentInParent<Rigidbody>();
        sensorstate = PlayerState.Idle;
        //previousVelocity = Vector3.zero;
        //linearAcceleration = Vector3.zero;
        //angularVelocity = Vector3.zero;
        biasDrift = Vector3.zero;
    }

    // 센서 데이터 생성(Orientation)
    void UpdateOrientation()
    {
        // 유니티 좌표를 ROS에서 사용하는 좌표로 변환
        orientation.x = -playerRigidBody.transform.rotation.z;
        orientation.y = playerRigidBody.transform.rotation.x;
        orientation.z = -playerRigidBody.transform.rotation.y;
        orientation.w = playerRigidBody.transform.rotation.w;
    }

    // 센서 데이터 생성(AngularVelocity)
    void UpdateAngularVelocity()
    {
        unityAngularVelocity = playerRigidBody.angularVelocity;

        // 유니티 좌표를 ROS에서 사용하는 좌표로 변환
        angularVelocity.x    = -unityAngularVelocity.z;
        angularVelocity.y    = unityAngularVelocity.x;
        angularVelocity.z    = -unityAngularVelocity.y;
    }

    // 상태에 따른 중력 적용
    void CalculateGravity()
    {
        if(sensorstate != PlayerState.Jump)
            // 회전에 따른 중력 적용
            unityLinearAcceleration += playerRigidBody.transform.rotation * Physics.gravity;
    }

    // 센서 데이터 생성(LinearAcceleration)
    void UpdateLinearAcceleration()
    {
        // 가속도 계산
        Vector3 currentVelocity = playerRigidBody.velocity;
        unityLinearAcceleration = (currentVelocity - previousVelocity) / Time.fixedDeltaTime;
        previousVelocity = currentVelocity;

        // 중력 계산
        CalculateGravity();
        // 유니티 좌표를 ROS에서 사용하는 좌표로 변환
        linearAcceleration.x = unityLinearAcceleration.z;
        linearAcceleration.y = -unityLinearAcceleration.x;
        linearAcceleration.z = unityLinearAcceleration.y;
    }

    // 노이즈 데이터 생성(Gaussian)
    Vector3 GetGaussianNoise()
    {
        float x = RandomGaussian() * gaussianNoiseStdDev;
        float y = RandomGaussian() * gaussianNoiseStdDev;
        float z = RandomGaussian() * gaussianNoiseStdDev;
        return new Vector3(x, y, z);
    }

    private float RandomGaussian()
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        return Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
    }

    // 노이즈 데이터 생성(Bias)
    Vector3 GetBiasDrift()
    {
        biasDrift += Random.insideUnitSphere * driftRate * Time.fixedDeltaTime;
        return biasDrift;
    }

    // 노이즈를 데이터에 추가
    void UpdateNoise()
    {
        Quaternion orientationNoise     = Quaternion.Euler(GetGaussianNoise());
        Vector3 angularVelNoise         = GetGaussianNoise();
        Vector3 linearAccNoise          = GetGaussianNoise();
        Vector3 linearAccBiasNoise      = GetBiasDrift();

        orientation *= orientationNoise;
        angularVelocity += angularVelNoise;
        linearAcceleration += (linearAccNoise + linearAccBiasNoise);
    }

    // 센서 데이터 업데이트
    void UpdateSensorData()
    {
        if (playerRigidBody == null) return;
        UpdateOrientation();
        UpdateAngularVelocity();
        UpdateLinearAcceleration();
        UpdateNoise();
    }

    // 센서의 상태 변환
    public void setState(PlayerState state)
    {
        sensorstate = state;
    }

    void FixedUpdate()
    {
        UpdateSensorData();
    }
}

