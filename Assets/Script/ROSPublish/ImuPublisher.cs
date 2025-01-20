using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

public class ImuPublisher : MonoBehaviour
{
    [Header("Unity Publish Setting")]
    [SerializeField]
    private ROSConnection   rosConnection;
    [SerializeField]
    private ImuSensor       imuSensor;
    [SerializeField]
    private string          topicName;

    private ImuMsg          imuMessage;

    void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        if(!string.IsNullOrEmpty(topicName)) 
            rosConnection.RegisterPublisher<ImuMsg>(topicName);
        
    }

    // imu 센서 데이터 받아오기
    public void GetData()
    {
        imuMessage = new ImuMsg();
        
        imuMessage.orientation.x = imuSensor.orientation.x;
        imuMessage.orientation.y = imuSensor.orientation.y;
        imuMessage.orientation.z = imuSensor.orientation.z;
        imuMessage.orientation.w = imuSensor.orientation.w;

        imuMessage.angular_velocity.x = imuSensor.angularVelocity.x;
        imuMessage.angular_velocity.y = imuSensor.angularVelocity.y;
        imuMessage.angular_velocity.z = imuSensor.angularVelocity.z;

        imuMessage.linear_acceleration.x = imuSensor.linearAcceleration.x;
        imuMessage.linear_acceleration.y = imuSensor.linearAcceleration.y;
        imuMessage.linear_acceleration.z = imuSensor.linearAcceleration.z;
    }

    // imu 데이터 발행
    void PublishData()
    {
        GetData();
        rosConnection.Publish(topicName, imuMessage);
        imuMessage = null;
    }

    void FixedUpdate()
    {
        if (rosConnection == null || string.IsNullOrEmpty(topicName) || imuSensor == null) return;
        PublishData();
    }
}
