using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using TMPro;

public class ImuSubsciber : MonoBehaviour
{
    [Header("Unity Subscribe Setting")]
    [SerializeField]
    private ROSConnection   rosConnection;
    [SerializeField]
    private ImuSensor       imuSensor;
    [SerializeField]
    private string          topicName;

    private Quaternion      orientation;
    private Vector3         angularVelocity;
    private Vector3         linearAcceleration;

    void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        rosConnection.Subscribe<ImuMsg>(topicName, ReceiveImuData);
    }

    void ReceiveImuData(ImuMsg imuMessage)
    {
        // orientation 받아오는 부분
        orientation = new Quaternion(
            (float)imuMessage.orientation.x,
            (float)imuMessage.orientation.y,
            (float)imuMessage.orientation.z,
            (float)imuMessage.orientation.w
        );

        // 각속도 받아오는 부분
        angularVelocity = new Vector3(
            (float)imuMessage.angular_velocity.x,
            (float)imuMessage.angular_velocity.y,
            (float)imuMessage.angular_velocity.z
        );

        // 선형 가속도 받아오는 부분
        linearAcceleration = new Vector3(
            (float)imuMessage.linear_acceleration.x,
            (float)imuMessage.linear_acceleration.y,
            (float)imuMessage.linear_acceleration.z
        );

    }

    // 받아온 센서데이터 센서에 전달
    void UpdateSensorData()
    {
        imuSensor.orientation = orientation;
        imuSensor.angularVelocity = angularVelocity;
        imuSensor.linearAcceleration = linearAcceleration;
    }

    void Update()
    {
        if (imuSensor == null) return;

        UpdateSensorData();
    }
}

