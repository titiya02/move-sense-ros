using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    // �繰�� �浹�ϴ��� �ϴ� ���� Ȯ���ϴ� ����
    private LayerMask       labWareDeskLayer;

    // ROS�� ���� �浹���� ���͸���Ʈ
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
            // �浹ü�� ���̾ Ư�� ���̾��϶���(���⼭�� LabWare ���̾)
            if (((1 << contact.otherCollider.gameObject.layer) & labWareDeskLayer) != 0)
            {
                // �浹������ �ߺ��� �����ʵ���
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
