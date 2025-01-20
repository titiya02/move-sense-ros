using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    [Header("Object")]
    [SerializeField]
    private GameObject target;
    private Vector3 forwardVector;

    // 카메라의 위치를 타겟의 전방의 일정거리 앞으로 설정
    void Awake()
    {
        forwardVector = target.transform.forward * 1.3f;
        transform.position += forwardVector;
    }

    // 카메라가 타겟을 전방의 일정 거리에서 계속 주시하도록
    void Update()
    {
        transform.position = target.transform.position + forwardVector;
        transform.LookAt(target.transform, transform.up);
    }
}
