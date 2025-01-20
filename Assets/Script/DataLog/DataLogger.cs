using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 데이터 확인용 스크립트
public class DataLogger : MonoBehaviour
{
    // 저장하고 싶은 데이터
    [Header("Data")]
    [SerializeField]
    private Player player;
    [SerializeField]
    private ImuSensor imusensor;
    [SerializeField]
    private PlayerMovement playerMovement;

    // 데이터 확인하기 위한 저장 스크립트
    private List<string> dataList = new List<string>();
    private string filePath;

    void Awake()
    {
        filePath = Application.dataPath + "/playerData.csv";
        //            시간,x좌표,y좌표,z좌표,x가속도,y가속도,z가속도,각속도,선형속도
        dataList.Add("Time,Pos_x,Pos_y,Pos_z,Acc_x,Acc_y,Acc_z,AngVel,Velocity");
    }

    // 물리엔진 관련 데이터 저장을 위해 FixedUpdate에서 (ROS 사용 될 기준으로 데이터 저장)
    void FixedUpdate()
    {
        string currentTime = Time.time.ToString("F2");
        string dataLine = $"{currentTime}," +
            // 플레이어 위치(GPS 대용)
            $"{player.transform.position.z:F2}," +
            $"{-player.transform.position.x:F2}," +
            $"{player.transform.position.y:F2}," +
            // IMU 센서 데이터
            $"{imusensor.linearAcceleration.x:F2}," +
            $"{-imusensor.linearAcceleration.y:F2}," +
            $"{imusensor.linearAcceleration.z:F2}," +
            $"{imusensor.angularVelocity.z:F2}," +
            // 플레이어의 속도
            $"{playerMovement.velocity:F2}";

        dataList.Add(dataLine);
    }

    // 시뮬레이션이 종료될때 데이터를 액셀로 저장
    private void OnApplicationQuit()
    {
        File.WriteAllLines(filePath, dataList);
    }
}
