using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private GameObject target;
    
    private void SetTarget(GameObject target)
    {
        this.target = target;
    }

    private void CameraFollowMode()
    {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y + 8, target.transform.position.z);
    }

    // 플레이어를 타겟으로 설정
    void Awake()
    {
        SetTarget(player);
    }

    // 플레이어를 따라다니도록 카메라 설정
    void Update()
    {
        CameraFollowMode();
    }
}
