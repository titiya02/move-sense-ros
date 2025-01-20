using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class PositionPublisher : MonoBehaviour
{
    [Header("Unity Publish Setting")]
    [SerializeField]
    private ROSConnection   rosConnection;
    [SerializeField]
    private Player          player;
    [SerializeField]
    private string          topicName;

    void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        if (!string.IsNullOrEmpty(topicName))
            rosConnection.RegisterPublisher<PointMsg>(topicName);
    }

    // 데이터 발행
    void PublishData()
    {
        
        Vector3 position = player.transform.position;

        PointMsg positionMsg = new PointMsg
        {
            x = position.z,
            y = -position.x,
            z = position.y
        };

        rosConnection.Publish(topicName, positionMsg);
    }

    void FixedUpdate()
    {
        if (rosConnection == null || player == null || string.IsNullOrEmpty(topicName)) return;
        PublishData();
    }
}
