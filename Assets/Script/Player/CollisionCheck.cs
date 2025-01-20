using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    // 사물에 충돌하는지 하는 것을 확인하는 변수
    private LayerMask       labWareDeskLayer;

    // ROS로 보낼 충돌관련 벡터리스트
    [SerializeField]
    private List<Vector3> collisionPoints = new List<Vector3>();

    [HideInInspector]
    public bool isSended = false;
    [HideInInspector]
    public bool ischecked = false;
    public List<Vector3> CollisionPoints => collisionPoints;

    void Awake()
    {
        labWareDeskLayer = LayerMask.GetMask("LabWare");
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // 충돌체의 레이어가 특정 레이어일때만(여기서는 LabWare 레이어만)
            if (((1 << contact.otherCollider.gameObject.layer) & labWareDeskLayer) != 0)
            {
                // 충돌지점이 중복이 되지않도록
                if (!collisionPoints.Contains(contact.point))
                {
                    collisionPoints.Add(contact.point);
                }
            }
        }
        ischecked = true;
    }

    private void FixedUpdate()
    {
        if(isSended)
        {
            collisionPoints.Clear();
            isSended = false;
        }
    }

}
