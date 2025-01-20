using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudVisualization : MonoBehaviour
{
    [Header("Subscriber")]
    [SerializeField]
    private VelodyneSubscriber  velodyneSub;

    [SerializeField]
    private GameObject          pointCloudPrefab;                       // Ŭ���� ������ ������

    private List<Vector3>       pointLists = new List<Vector3>();       // �޾ƿ� ����Ʈ Ŭ���� ������
    private List<GameObject>    cloudPrefabs = new List<GameObject>();  // Ŭ���� ������ �����ͷ� ������ �����͵�

    private float               drawFrequency = 0.1f; // ������ �׸��� �ֱ�
    private float               currentTime;


    private void Awake()
    {
        currentTime = Time.time;
    }

    // ������ �޾ƿ��� �Լ�
    void GetData()
    {
        if (velodyneSub == null) return;
        pointLists = velodyneSub.PointLists;
    }

    // �޾ƿ� �����͸� �׸��� �Լ�
    void DrawPointCloud()
    {
        if (pointCloudPrefab == null) return;
        if(Time.time - currentTime >= drawFrequency)
        {
            GetData();

            // ó�� ������ ũ�Ⱑ �����Ƿ� ����
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
                // ����Ʈ�� Ŭ������ ������ ����
                for(int i = 0; i < Mathf.Max(pointLists.Count, cloudPrefabs.Count); ++i)
                {
                    if(i >= cloudPrefabs.Count) // ����Ʈ�� Ŭ���� �����պ��� ������ �߰� ����
                    {
                        GameObject obj = Instantiate<GameObject>(pointCloudPrefab, pointLists[i] + transform.position, transform.rotation, transform);
                        cloudPrefabs.Add(obj);
                    }
                    else if (i >= pointLists.Count) // Ŭ���� �������� ����Ʈ���� ������ �߰��κ� ��Ȱ��ȭ
                    {
                        cloudPrefabs[i].SetActive(false);
                    }
                    else // �׿ܿ��� ��ġ ������Ʈ
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
