using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// ������ Ȯ�ο� ��ũ��Ʈ
public class DataLogger : MonoBehaviour
{
    // �����ϰ� ���� ������
    [Header("Data")]
    [SerializeField]
    private Player player;
    [SerializeField]
    private ImuSensor imusensor;
    [SerializeField]
    private PlayerMovement playerMovement;

    // ������ Ȯ���ϱ� ���� ���� ��ũ��Ʈ
    private List<string> dataList = new List<string>();
    private string filePath;

    void Awake()
    {
        filePath = Application.dataPath + "/playerData.csv";
        //            �ð�,x��ǥ,y��ǥ,z��ǥ,x���ӵ�,y���ӵ�,z���ӵ�,���ӵ�,�����ӵ�
        dataList.Add("Time,Pos_x,Pos_y,Pos_z,Acc_x,Acc_y,Acc_z,AngVel,Velocity");
    }

    // �������� ���� ������ ������ ���� FixedUpdate���� (ROS ��� �� �������� ������ ����)
    void FixedUpdate()
    {
        string currentTime = Time.time.ToString("F2");
        string dataLine = $"{currentTime}," +
            // �÷��̾� ��ġ(GPS ���)
            $"{player.transform.position.z:F2}," +
            $"{-player.transform.position.x:F2}," +
            $"{player.transform.position.y:F2}," +
            // IMU ���� ������
            $"{imusensor.linearAcceleration.x:F2}," +
            $"{-imusensor.linearAcceleration.y:F2}," +
            $"{imusensor.linearAcceleration.z:F2}," +
            $"{imusensor.angularVelocity.z:F2}," +
            // �÷��̾��� �ӵ�
            $"{playerMovement.velocity:F2}";

        dataList.Add(dataLine);
    }

    // �ùķ��̼��� ����ɶ� �����͸� �׼��� ����
    private void OnApplicationQuit()
    {
        File.WriteAllLines(filePath, dataList);
    }
}
