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

    // ������ ����
    void GetData(PointCloud2Msg msg)
    {
        int pointCount = msg.data.Length / (int)msg.point_step; // ������ ����Ʈ�� ����

        // ���� ����Ʈ�� ũ�Ⱑ ������ �����ͺ��� ������ ũ�� Ȯ��
        if (pointLists.Count < pointCount)
        {
            while (pointLists.Count < pointCount)
            {
                pointLists.Add(Vector3.zero); // ���� ����Ʈ ũ�⺸�� ū �����Ͱ� ������ �� ������ �߰�
            }
        }
        // ���� ����Ʈ�� ũ�Ⱑ ������ �����ͺ��� ũ�� ũ�� ���
        else if (pointLists.Count > pointCount)
        {
            pointLists.RemoveRange(pointCount, pointLists.Count - pointCount); // �� ���� �����Ͱ� ���� ��� ���
        }

        // ����Ʈ ������ ������Ʈ
        for (int i = 0; i < pointCount; i++)
        {
            // x, y, z ��ǥ ����
            point.z = System.BitConverter.ToSingle(msg.data, i * (int)msg.point_step + (int)msg.fields[0].offset);
            point.x = -System.BitConverter.ToSingle(msg.data, i * (int)msg.point_step + (int)msg.fields[1].offset);
            point.y = System.BitConverter.ToSingle(msg.data, i * (int)msg.point_step + (int)msg.fields[2].offset);
            pointLists[i] = point; // ����Ʈ�� �ִ� ���� �����͸� ������Ʈ
        }
    }
}
