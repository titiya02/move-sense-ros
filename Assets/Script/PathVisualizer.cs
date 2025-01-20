using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private Player          player;
    private LineRenderer    lineRenderer;
    private float           updateInterval = 0.1f;
    private float           minimumDistance = 0.1f;

    private List<Vector3> pathPoints = new List<Vector3>();
    private Vector3 previousPosition = new Vector3();
    private float time = 0;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    // 이동 경로 생성
    void CreateLine()
    {
        Vector3 point = player.transform.position;
        point.y += 1;
        // 일정 거리를 이동하지않으면 경로 포인트 추가 X
        if(Vector3.Distance(point, previousPosition) >= minimumDistance)
        {
            pathPoints.Add(point);

            lineRenderer.positionCount = pathPoints.Count;
            lineRenderer.SetPositions(pathPoints.ToArray());
        }
        
        previousPosition = point;
    }

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if(time >= updateInterval)
        {
            CreateLine();
            time = 0;
        }
    }
}
