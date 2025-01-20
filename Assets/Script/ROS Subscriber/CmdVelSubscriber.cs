using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using TMPro;

public class CmdVelSubscriber : MonoBehaviour
{
    [Header("Unity Subscribe Setting")]
    [SerializeField]
    private ROSConnection   rosConnection;
    [SerializeField]
    private string          cmdVelTopic;

    // 받아올 속도 저장용 변수
    private Vector3         linearVelocity;
    private Vector3         angularVelocity;

    // 받아온 변수 외부 스크립트에서 사용할수 있도록 Get 설정
    public Vector3          LinearVelocity => linearVelocity;
    public Vector3          AngularVelocity => angularVelocity;

    // ROS에서 데이터 발행되면 구독하도록 설정
    void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        rosConnection.Subscribe<TwistMsg>(cmdVelTopic, ReceiveCmdMsg);
    }

    // ROS에서 구독한 데이터 저장하는 함수
    void ReceiveCmdMsg(TwistMsg cmdMsg)
    {
        linearVelocity = new Vector3(
            -(float)cmdMsg.linear.y,
            (float)cmdMsg.linear.z,
            (float)cmdMsg.linear.x
        );

        angularVelocity = new Vector3(
            (float)cmdMsg.angular.y,
            -(float)cmdMsg.angular.z,
            -(float)cmdMsg.angular.x
        );
    }

}
