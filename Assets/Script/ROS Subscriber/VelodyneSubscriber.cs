using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

public class VelodyneSubscriber : MonoBehaviour
{
    [Header("Unity Subscribe Setting")]
    [SerializeField]
    private ROSConnection   rosConnection;
    [SerializeField]
    private string          topicName;

    private List<Vector3>   pointLists = new List<Vector3>();
    private Vector3         point;

    public List<Vector3>    PointLists => pointLists;

    private void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        rosConnection.Subscribe<PointCloud2Msg>(topicName, GetData);
        point = new Vector3();
    }

    // 데이터 구독
    void GetData(PointCloud2Msg msg)
    {
        int pointCount = msg.data.Length / (int)msg.point_step; // 들어오는 포인트의 개수

        // 현재 리스트의 크기가 들어오는 데이터보다 작으면 크기 확장
        if (pointLists.Count < pointCount)
        {
            while (pointLists.Count < pointCount)
            {
                pointLists.Add(Vector3.zero); // 기존 리스트 크기보다 큰 데이터가 들어오면 빈 공간을 추가
            }
        }
        // 현재 리스트의 크기가 들어오는 데이터보다 크면 크기 축소
        else if (pointLists.Count > pointCount)
        {
            pointLists.RemoveRange(pointCount, pointLists.Count - pointCount); // 더 많은 데이터가 있을 경우 축소
        }

        // 포인트 데이터 업데이트
        for (int i = 0; i < pointCount; i++)
        {
            // x, y, z 좌표 추출
            point.z = System.BitConverter.ToSingle(msg.data, i * (int)msg.point_step + (int)msg.fields[0].offset);
            point.x = -System.BitConverter.ToSingle(msg.data, i * (int)msg.point_step + (int)msg.fields[1].offset);
            point.y = System.BitConverter.ToSingle(msg.data, i * (int)msg.point_step + (int)msg.fields[2].offset);
            pointLists[i] = point; // 리스트에 있는 기존 데이터를 업데이트
        }
    }
}
