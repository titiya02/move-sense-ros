using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObjectRotation : MonoBehaviour
{
    [Header("Object")]
    [SerializeField]
    private GameObject targetObject;
    
    // 타겟의 회전상태를 현재 오브젝트에 적용
    void Update()
    {
        transform.rotation = targetObject.transform.rotation;
    }
}
