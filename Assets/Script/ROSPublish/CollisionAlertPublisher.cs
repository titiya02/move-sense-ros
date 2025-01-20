using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class CollisionAlertPublisher : MonoBehaviour
{
    [Header("Unity Publish Setting")]
    [SerializeField]
    private ROSConnection   rosConnection;
    [SerializeField]
    private CollisionCheck  player;
    [SerializeField]
    private string          topicName;

    void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        if (!string.IsNullOrEmpty(topicName))
            rosConnection.RegisterPublisher<Vector3Msg>(topicName);
    }

    // 데이터 발행
    void PublishData()
    {
        // 충돌하고있는 책상류가 없으면 (0,0,0)을 반환
        if(player.CollisionPoints.Count == 0)
        {
            Vector3Msg rosConnectionPoint = new Vector3Msg
            {
                x = 0,
                y = 0,
                z = 0
            };

            rosConnection.Publish(topicName, rosConnectionPoint);
        }
        // 충돌이 발생하면 충돌한 부분을 전부 발행
        else
        {
            for(int i = 0; i < player.CollisionPoints.Count; ++i)
            {
                Vector3Msg rosConnectionPoint = new Vector3Msg
                {
                    x = player.CollisionPoints[i].z,
                    y = -player.CollisionPoints[i].x,
                    z = player.CollisionPoints[i].y
                };
                rosConnection.Publish(topicName, rosConnectionPoint);
            }
        }
    }

    private void FixedUpdate()
    {
        if (rosConnection == null || string.IsNullOrEmpty(topicName) || player == null) return;
        if (player.ischecked)
        {
            PublishData();
            player.isSended = true;
            player.ischecked = false;
        }
    }
    
}
