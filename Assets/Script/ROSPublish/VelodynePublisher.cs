using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

public class VelodynePublisher : MonoBehaviour
{
    [Header("Unity Publish Setting")]
    [SerializeField]
    private ROSConnection   rosConnection;
    [SerializeField]
    private VelodyneSensor  velodyneSensor;
    [SerializeField]
    private string          topicName;
    [SerializeField]
    private float           publishInterval;

    // ����Ʈ Ŭ���� ������
    private Vector3[]       points = {};
    private float           lastPublishTime;

    void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        if (!string.IsNullOrEmpty(topicName))
            rosConnection.RegisterPublisher<PointCloud2Msg>(topicName);
        lastPublishTime = Time.time;
    }

    // �޼��� ����
    void PublishMsg()
    {
        // VelodyneSensor���� ����Ʈ ������ ��������
        points = velodyneSensor.Points;

        PointCloud2Msg pointCloudMsg = new PointCloud2Msg();

        // Header ����
        HeaderMsg header = new HeaderMsg
        {
            stamp = new TimeMsg
            {
                sec = Mathf.FloorToInt(Time.time),
                nanosec = (uint)((Time.time - Mathf.Floor(Time.time)) * 1e9)
            },
            frame_id = "velodyne" // ���̴� ������ ��ǥ�� ID
        };

        pointCloudMsg.header = header;
        pointCloudMsg.height = 1; // 1D �迭
        pointCloudMsg.width = (uint)points.Length; // ����Ʈ ����

        // ����Ʈ �������� �ʵ� ���� (x, y, z)
        pointCloudMsg.fields = new PointFieldMsg[]
        {
            new PointFieldMsg() { name = "x", offset = 0, datatype = PointFieldMsg.FLOAT32, count = 1 },
            new PointFieldMsg() { name = "y", offset = 4, datatype = PointFieldMsg.FLOAT32, count = 1 },
            new PointFieldMsg() { name = "z", offset = 8, datatype = PointFieldMsg.FLOAT32, count = 1 },
            new PointFieldMsg() { name = "intensity", offset = 12, datatype = PointFieldMsg.FLOAT32, count = 1 } // ���� �߰�
        };

        pointCloudMsg.is_bigendian = false; // Little Endian
        pointCloudMsg.point_step = 16; // �� ����Ʈ�� 16����Ʈ (x, y, z, intensity)
        pointCloudMsg.row_step = pointCloudMsg.point_step * pointCloudMsg.width;

        // ����Ʈ �����͸� ByteArray�� ��ȯ
        List<byte> data = new List<byte>();

        foreach (Vector3 point in points)
        {
            // x, y, z�� ����Ʈ �迭�� ��ȯ�ؼ� �߰�
            data.AddRange(System.BitConverter.GetBytes(point.x));
            data.AddRange(System.BitConverter.GetBytes(point.y));
            data.AddRange(System.BitConverter.GetBytes(point.z));
            data.AddRange(System.BitConverter.GetBytes(1.0f)); // ���� 1.0f (���� ��)
        }

        pointCloudMsg.data = data.ToArray(); // ����Ʈ �迭�� ��ȯ�Ͽ� �޽����� ����
        pointCloudMsg.is_dense = true; // ��� ����Ʈ�� ��ȿ

        // ROS�� �޽��� ����
        rosConnection.Publish(topicName, pointCloudMsg);
    }

    void FixedUpdate()
    {
        if (rosConnection == null || string.IsNullOrEmpty(topicName) || velodyneSensor == null) return;
        // �ֱ������� �޽��� ����
        if (Time.time - lastPublishTime >= publishInterval)
        {
            lastPublishTime = Time.time;
            PublishMsg();
        }
    }
}
