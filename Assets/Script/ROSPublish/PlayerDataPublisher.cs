using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;

enum DataType { Calorie, Velocity };
public class PlayerDataPublisher : MonoBehaviour
{
    [Header("Unity Publish Setting")]
    [SerializeField]
    private ROSConnection   rosConnection;
    [SerializeField]
    private string          topicName;
    [SerializeField]
    private Player          player;
    [SerializeField]
    private DataType        type;

    // 데이터를 참조할 스크립트변수
    private PlayerCalorie   playerCalorie;
    private PlayerMovement  playerMovement;

    void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        if (!string.IsNullOrEmpty(topicName))
            rosConnection.RegisterPublisher<Float32Msg>(topicName);

        if (player != null)
        {
            playerCalorie = player.GetComponent<PlayerCalorie>();
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    // 데이터 발행
    void PublishData()
    {
        Float32Msg dataToSend;
        switch (type)
        {
            case DataType.Calorie:
                if (playerCalorie != null)
                    dataToSend = new Float32Msg(playerCalorie.UsedCalorie);
                else return;
                break;
            case DataType.Velocity:
                if (playerMovement != null)
                    dataToSend = new Float32Msg(playerMovement.velocity);
                else return;
                break;
            default:
                return;
        }
        
        rosConnection.Publish(topicName, dataToSend);
    }

    void FixedUpdate()
    {
        if (rosConnection == null || string.IsNullOrEmpty(topicName) || player == null) return;
        PublishData();
    }
}
