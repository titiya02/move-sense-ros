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

    // �޾ƿ� �ӵ� ����� ����
    private Vector3         linearVelocity;
    private Vector3         angularVelocity;

    // �޾ƿ� ���� �ܺ� ��ũ��Ʈ���� ����Ҽ� �ֵ��� Get ����
    public Vector3          LinearVelocity => linearVelocity;
    public Vector3          AngularVelocity => angularVelocity;

    // ROS���� ������ ����Ǹ� �����ϵ��� ����
    void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        rosConnection.Subscribe<TwistMsg>(cmdVelTopic, ReceiveCmdMsg);
    }

    // ROS���� ������ ������ �����ϴ� �Լ�
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
