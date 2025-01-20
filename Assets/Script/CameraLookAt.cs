using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    [Header("Object")]
    [SerializeField]
    private GameObject target;
    private Vector3 forwardVector;

    // ī�޶��� ��ġ�� Ÿ���� ������ �����Ÿ� ������ ����
    void Awake()
    {
        forwardVector = target.transform.forward * 1.3f;
        transform.position += forwardVector;
    }

    // ī�޶� Ÿ���� ������ ���� �Ÿ����� ��� �ֽ��ϵ���
    void Update()
    {
        transform.position = target.transform.position + forwardVector;
        transform.LookAt(target.transform, transform.up);
    }
}
