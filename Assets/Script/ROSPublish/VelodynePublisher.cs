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

    // 포인트 클라우드 데이터
    private Vector3[]       points = {};
    private float           lastPublishTime;

    void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        if (!string.IsNullOrEmpty(topicName))
            rosConnection.RegisterPublisher<PointCloud2Msg>(topicName);
        lastPublishTime = Time.time;
    }

    // 메세지 발행
    void PublishMsg()
    {
        // VelodyneSensor에서 포인트 데이터 가져오기
        points = velodyneSensor.Points;

        PointCloud2Msg pointCloudMsg = new PointCloud2Msg();

        // Header 설정
        HeaderMsg header = new HeaderMsg
        {
            stamp = new TimeMsg
            {
                sec = Mathf.FloorToInt(Time.time),
                nanosec = (uint)((Time.time - Mathf.Floor(Time.time)) * 1e9)
            },
            frame_id = "velodyne" // 라이다 센서의 좌표계 ID
        };

        pointCloudMsg.header = header;
        pointCloudMsg.height = 1; // 1D 배열
        pointCloudMsg.width = (uint)points.Length; // 포인트 개수

        // 포인트 데이터의 필드 설정 (x, y, z)
        pointCloudMsg.fields = new PointFieldMsg[]
        {
            new PointFieldMsg() { name = "x", offset = 0, datatype = PointFieldMsg.FLOAT32, count = 1 },
            new PointFieldMsg() { name = "y", offset = 4, datatype = PointFieldMsg.FLOAT32, count = 1 },
            new PointFieldMsg() { name = "z", offset = 8, datatype = PointFieldMsg.FLOAT32, count = 1 },
            new PointFieldMsg() { name = "intensity", offset = 12, datatype = PointFieldMsg.FLOAT32, count = 1 } // 강도 추가
        };

        pointCloudMsg.is_bigendian = false; // Little Endian
        pointCloudMsg.point_step = 16; // 각 포인트가 16바이트 (x, y, z, intensity)
        pointCloudMsg.row_step = pointCloudMsg.point_step * pointCloudMsg.width;

        // 포인트 데이터를 ByteArray로 변환
        List<byte> data = new List<byte>();

        foreach (Vector3 point in points)
        {
            // x, y, z를 바이트 배열로 변환해서 추가
            data.AddRange(System.BitConverter.GetBytes(point.x));
            data.AddRange(System.BitConverter.GetBytes(point.y));
            data.AddRange(System.BitConverter.GetBytes(point.z));
            data.AddRange(System.BitConverter.GetBytes(1.0f)); // 강도 1.0f (예제 값)
        }

        pointCloudMsg.data = data.ToArray(); // 바이트 배열로 변환하여 메시지에 저장
        pointCloudMsg.is_dense = true; // 모든 포인트가 유효

        // ROS로 메시지 전송
        rosConnection.Publish(topicName, pointCloudMsg);
    }

    void FixedUpdate()
    {
        if (rosConnection == null || string.IsNullOrEmpty(topicName) || velodyneSensor == null) return;
        // 주기적으로 메시지 발행
        if (Time.time - lastPublishTime >= publishInterval)
        {
            lastPublishTime = Time.time;
            PublishMsg();
        }
    }
}
