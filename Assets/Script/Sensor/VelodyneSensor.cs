using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Sensor;

public class VelodyneSensor : MonoBehaviour
{
    private int             laserCount = 16;              // 레이저 개수
    private float           minVerticalAngle = -15f;      // 수직 시야각 최솟값
    private float           maxVerticalAngle = 15f;       // 수직 시야각 최댓값
    private float           horizontalResolution = 1.5f;  // 수평 해상도 (각도 간격)
    private float           maxDistance = 40.0f;          // 최대 레이저 범위

    private int             layerToIgnore;
    private int             layerMask;

    private float           scanFrequency = 0.1f;       // 스캔 주기
    private float           currentTime;

    private float           currentAngle = 0f;

    private Vector3[]       points;
    private List<Vector3>   pointList = new List<Vector3>(); // 모든 포인트를 저장할 리스트

    // ROS에서 rviz로 Velodyne 데이터 시각화할때 보기 편하기 위한 스케일
    private float           rvizScale = 0.1f;

    public Vector3[]        Points => pointList.ToArray(); // 전체 포인트를 반환하도록 수정
    
    private void Awake()
    {
        points = new Vector3[laserCount];
        currentTime = Time.time;
        layerToIgnore = LayerMask.NameToLayer("Player"); // Raycast 할때 무시하고 싶은 레이어
        layerMask = ~(1 << layerToIgnore);  // 무시하고 싶은 레이어 제외 나머지 레이어 
    }

    // 360도 돌면서 주변환경 스캔
    void RayScan()
    {
        while(currentAngle < 360.0f)
        {
            currentAngle += horizontalResolution;
            MakeRay();
        }
        currentAngle = 0;

    }

    // 주변 환경을 스캔하는 Ray 생성
    void MakeRay()
    {
        // 각 레이저 빔에 대해 수직 시야각 계산 및 레이캐스트
        for (int i = 0; i < laserCount; i++)
        {
            // 수직 각도 계산
            float verticalAngle = Mathf.Lerp(minVerticalAngle, maxVerticalAngle, i / (float)(laserCount - 1));

            // 수직 및 수평 각도 적용
            Quaternion rotation = Quaternion.Euler(verticalAngle, currentAngle, 0);
            Vector3 direction = rotation * Vector3.forward;

            // Raycast로 레이저 빔 발사
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            // 충돌한 경우
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                // 포인트 클라우드 데이터로 처리 가능
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
                points[i].x = hit.point.z * rvizScale;
                points[i].y = -hit.point.x * rvizScale;
                points[i].z = hit.point.y * rvizScale;
                pointList.Add(points[i]); // 충돌한 지점 추가
            }
            // 충돌하지 않은 경우
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green);
                points[i].x = (ray.origin + direction * maxDistance).z * rvizScale;
                points[i].y = -(ray.origin + direction * maxDistance).x * rvizScale;
                points[i].z = (ray.origin + direction * maxDistance).y * rvizScale;
                pointList.Add(points[i]);
            }
        }
    }

    void Update()
    {
        // 스캔하기 전에 리스트 초기화
        if(Time.time - currentTime >= scanFrequency)
        {
            currentTime = Time.time;
            pointList.Clear();
            RayScan();
        }
    }
}
