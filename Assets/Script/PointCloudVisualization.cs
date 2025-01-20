using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudVisualization : MonoBehaviour
{
    [Header("Subscriber")]
    [SerializeField]
    private VelodyneSubscriber  velodyneSub;

    [SerializeField]
    private GameObject          pointCloudPrefab;                       // 클라우드 프리펩 데이터

    private List<Vector3>       pointLists = new List<Vector3>();       // 받아온 포인트 클라우드 데이터
    private List<GameObject>    cloudPrefabs = new List<GameObject>();  // 클라우드 프리펩 데이터로 생성한 포인터들

    private float               drawFrequency = 0.1f; // 데이터 그리는 주기
    private float               currentTime;


    private void Awake()
    {
        currentTime = Time.time;
    }

    // 데이터 받아오는 함수
    void GetData()
    {
        if (velodyneSub == null) return;
        pointLists = velodyneSub.PointLists;
    }

    // 받아온 데이터를 그리는 함수
    void DrawPointCloud()
    {
        if (pointCloudPrefab == null) return;
        if(Time.time - currentTime >= drawFrequency)
        {
            GetData();

            // 처음 받으면 크기가 없으므로 생성
            if (cloudPrefabs.Count == 0)
            {
                foreach (Vector3 point in pointLists)
                {
                    GameObject obj = Instantiate<GameObject>(pointCloudPrefab, point + transform.position, transform.rotation, transform);
                    cloudPrefabs.Add(obj);
                }
            }
            else
            {
                // 포인트나 클라우드중 많은쪽 선택
                for(int i = 0; i < Mathf.Max(pointLists.Count, cloudPrefabs.Count); ++i)
                {
                    if(i >= cloudPrefabs.Count) // 포인트가 클라우드 프리팹보다 많으면 추가 생성
                    {
                        GameObject obj = Instantiate<GameObject>(pointCloudPrefab, pointLists[i] + transform.position, transform.rotation, transform);
                        cloudPrefabs.Add(obj);
                    }
                    else if (i >= pointLists.Count) // 클라우드 프리팹이 포인트보다 많으면 추가부분 비활성화
                    {
                        cloudPrefabs[i].SetActive(false);
                    }
                    else // 그외에는 위치 업데이트
                    {
                        cloudPrefabs[i].transform.position = pointLists[i] + transform.position;
                        cloudPrefabs[i].SetActive(true);
                    }
                }
            }
            currentTime = Time.time;
        }
    }

    private void LateUpdate()
    {
        DrawPointCloud();
    }
}
