using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { Idle, Walk, Run, Jump };
public class Player : MonoBehaviour
{
    private Rigidbody       playerRigidbody;
    private PlayerMovement  playerMovement;
    private PlayerCalorie   playerCalorie;
    private ImuSensor       rootImuSensor;

    private Animator        animator;

    // ���߿� �ִ��� üũ�� ����
    private Ray ray;
    private Vector3 rayOrigin;
    private float maxDistance = 0.3f;

    // RayCast�� ����
    private int layerToIgnore = 0;
    private int layerMask = 0;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerMovement  = GetComponent<PlayerMovement>();
        playerCalorie   = GetComponent<PlayerCalorie>();
        rootImuSensor   = GetComponentInChildren<ImuSensor>();
        animator        = GetComponent<Animator>();

        ray             = new Ray();
        rayOrigin       = new Vector3();

        layerToIgnore = LayerMask.GetMask("Player", "PlayerCollider");
        layerMask       = ~layerToIgnore;
    }

    // ���� ���¸� ����(�÷��̾��� ������ �����ϴ� �ٸ� ������Ʈ�� ����)
    private void SetStates(PlayerState state)
    {
        playerMovement.SetState(state);
        playerCalorie.SetState(state);
        rootImuSensor.setState(state);
    }

    // �ٴ����� ���ִ��� üũ
    private bool CheckStand()
    {
        ray.direction = Vector3.down;
        rayOrigin.x = playerRigidbody.transform.position.x;
        rayOrigin.y = playerRigidbody.transform.position.y + maxDistance / 2;
        rayOrigin.z = playerRigidbody.transform.position.z;

        ray.origin = rayOrigin;

        RaycastHit hit;

        // ���ִ� �ٴ� üũ
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            playerMovement.GetRayPoint(hit.point);
            return true;
        }
        playerMovement.GetRayPoint(transform.position);
        return false;
    }

    // ����Ƽ �Է� �����ӿ�
    private void UpdateAnimation()
    {
        if(!CheckStand())
            animator.SetBool("isJump", true);
        else
            animator.SetBool("isJump", false);

        animator.SetFloat("BasicMoveParameter", playerMovement.AnimationMovement);
    }

    // ���¾�����Ʈ
    private void UpdateState()
    {
        if (!CheckStand())
        {
            SetStates(PlayerState.Jump);
            playerRigidbody.drag = 0.2f;
        }
        else if (playerMovement.AnimationMovement > 0.51f)
        {
            SetStates(PlayerState.Run);
            playerRigidbody.drag = 0.7f;
        }
        else if (playerMovement.AnimationMovement > 0.01f)
        {
            SetStates(PlayerState.Walk);
            playerRigidbody.drag = 1.5f;
        }
        else if (playerMovement.AnimationMovement <= 0.01f)
        {
            SetStates(PlayerState.Idle);
            playerRigidbody.drag = 2.5f;
        }
    }

    // ROS �Է� �����ӿ�
    private void RosMove()
    {
        if (playerMovement.moveBack)
            animator.SetFloat("BasicMoveParameter", -playerMovement.AnimationMovement);
        else
            animator.SetFloat("BasicMoveParameter", playerMovement.AnimationMovement);
    }

    void FixedUpdate()
    {
        UpdateAnimation();
        UpdateState();
        // RosMove();
    }
}
