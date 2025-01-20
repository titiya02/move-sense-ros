using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ImuSensor : MonoBehaviour
{
    // ROS���� ���� ������
    [Header("Imu Sensor Parameter")]
    public Vector3 linearAcceleration;
    public Vector3 angularVelocity;
    public Quaternion orientation;

    // ����Ƽ���� ���� ������
    private Vector3 unityAngularVelocity;
    private Vector3 unityLinearAcceleration;

    // ���� ������ ����� ����
    private Rigidbody playerRigidBody;
    private Vector3 previousVelocity;
    private PlayerState sensorstate;

    // ������ ����
    private float gaussianNoiseStdDev = 0.05f; // ����þ� ������ ǥ������
    private float driftRate = 0.001f; // ���̾ �帮��Ʈ �ӵ�

    // ������ ��� ����
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

    // ���� ������ ����(Orientation)
    void UpdateOrientation()
    {
        // ����Ƽ ��ǥ�� ROS���� ����ϴ� ��ǥ�� ��ȯ
        orientation.x = -playerRigidBody.transform.rotation.z;
        orientation.y = playerRigidBody.transform.rotation.x;
        orientation.z = -playerRigidBody.transform.rotation.y;
        orientation.w = playerRigidBody.transform.rotation.w;
    }

    // ���� ������ ����(AngularVelocity)
    void UpdateAngularVelocity()
    {
        unityAngularVelocity = playerRigidBody.angularVelocity;

        // ����Ƽ ��ǥ�� ROS���� ����ϴ� ��ǥ�� ��ȯ
        angularVelocity.x    = -unityAngularVelocity.z;
        angularVelocity.y    = unityAngularVelocity.x;
        angularVelocity.z    = -unityAngularVelocity.y;
    }

    // ���¿� ���� �߷� ����
    void CalculateGravity()
    {
        if(sensorstate != PlayerState.Jump)
            // ȸ���� ���� �߷� ����
            unityLinearAcceleration += playerRigidBody.transform.rotation * Physics.gravity;
    }

    // ���� ������ ����(LinearAcceleration)
    void UpdateLinearAcceleration()
    {
        // ���ӵ� ���
        Vector3 currentVelocity = playerRigidBody.velocity;
        unityLinearAcceleration = (currentVelocity - previousVelocity) / Time.fixedDeltaTime;
        previousVelocity = currentVelocity;

        // �߷� ���
        CalculateGravity();
        // ����Ƽ ��ǥ�� ROS���� ����ϴ� ��ǥ�� ��ȯ
        linearAcceleration.x = unityLinearAcceleration.z;
        linearAcceleration.y = -unityLinearAcceleration.x;
        linearAcceleration.z = unityLinearAcceleration.y;
    }

    // ������ ������ ����(Gaussian)
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

    // ������ ������ ����(Bias)
    Vector3 GetBiasDrift()
    {
        biasDrift += Random.insideUnitSphere * driftRate * Time.fixedDeltaTime;
        return biasDrift;
    }

    // ����� �����Ϳ� �߰�
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

    // ���� ������ ������Ʈ
    void UpdateSensorData()
    {
        if (playerRigidBody == null) return;
        UpdateOrientation();
        UpdateAngularVelocity();
        UpdateLinearAcceleration();
        UpdateNoise();
    }

    // ������ ���� ��ȯ
    public void setState(PlayerState state)
    {
        sensorstate = state;
    }

    void FixedUpdate()
    {
        UpdateSensorData();
    }
}

