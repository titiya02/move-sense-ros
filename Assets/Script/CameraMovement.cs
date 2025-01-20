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

    // �÷��̾ Ÿ������ ����
    void Awake()
    {
        SetTarget(player);
    }

    // �÷��̾ ����ٴϵ��� ī�޶� ����
    void Update()
    {
        CameraFollowMode();
    }
}
