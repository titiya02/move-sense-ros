using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObjectRotation : MonoBehaviour
{
    [Header("Object")]
    [SerializeField]
    private GameObject targetObject;
    
    // Ÿ���� ȸ�����¸� ���� ������Ʈ�� ����
    void Update()
    {
        transform.rotation = targetObject.transform.rotation;
    }
}
