using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Sensor;

public class VelodyneSensor : MonoBehaviour
{
    private int             laserCount = 16;              // ������ ����
    private float           minVerticalAngle = -15f;      // ���� �þ߰� �ּڰ�
    private float           maxVerticalAngle = 15f;       // ���� �þ߰� �ִ�
    private float           horizontalResolution = 1.5f;  // ���� �ػ� (���� ����)
    private float           maxDistance = 40.0f;          // �ִ� ������ ����

    private int             layerToIgnore;
    private int             layerMask;

    private float           scanFrequency = 0.1f;       // ��ĵ �ֱ�
    private float           currentTime;

    private float           currentAngle = 0f;

    private Vector3[]       points;
    private List<Vector3>   pointList = new List<Vector3>(); // ��� ����Ʈ�� ������ ����Ʈ

    // ROS���� rviz�� Velodyne ������ �ð�ȭ�Ҷ� ���� ���ϱ� ���� ������
    private float           rvizScale = 0.1f;

    public Vector3[]        Points => pointList.ToArray(); // ��ü ����Ʈ�� ��ȯ�ϵ��� ����
    
    private void Awake()
    {
        points = new Vector3[laserCount];
        currentTime = Time.time;
        layerToIgnore = LayerMask.NameToLayer("Player"); // Raycast �Ҷ� �����ϰ� ���� ���̾�
        layerMask = ~(1 << layerToIgnore);  // �����ϰ� ���� ���̾� ���� ������ ���̾� 
    }

    // 360�� ���鼭 �ֺ�ȯ�� ��ĵ
    void RayScan()
    {
        while(currentAngle < 360.0f)
        {
            currentAngle += horizontalResolution;
            MakeRay();
        }
        currentAngle = 0;

    }

    // �ֺ� ȯ���� ��ĵ�ϴ� Ray ����
    void MakeRay()
    {
        // �� ������ ���� ���� ���� �þ߰� ��� �� ����ĳ��Ʈ
        for (int i = 0; i < laserCount; i++)
        {
            // ���� ���� ���
            float verticalAngle = Mathf.Lerp(minVerticalAngle, maxVerticalAngle, i / (float)(laserCount - 1));

            // ���� �� ���� ���� ����
            Quaternion rotation = Quaternion.Euler(verticalAngle, currentAngle, 0);
            Vector3 direction = rotation * Vector3.forward;

            // Raycast�� ������ �� �߻�
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            // �浹�� ���
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                // ����Ʈ Ŭ���� �����ͷ� ó�� ����
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
                points[i].x = hit.point.z * rvizScale;
                points[i].y = -hit.point.x * rvizScale;
                points[i].z = hit.point.y * rvizScale;
                pointList.Add(points[i]); // �浹�� ���� �߰�
            }
            // �浹���� ���� ���
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
        // ��ĵ�ϱ� ���� ����Ʈ �ʱ�ȭ
        if(Time.time - currentTime >= scanFrequency)
        {
            currentTime = Time.time;
            pointList.Clear();
            RayScan();
        }
    }
}
