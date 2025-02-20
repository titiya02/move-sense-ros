using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody        playerRigidbody;

    // ����Ƽ �������� �����ӿ� ������
    // �̵� ���� ����
    private Vector3          movement;
    private PlayerState      state;
    private float            animationMovement = 0;
    
    private float            walkSpeed = 3.0f;
    private float            runSpeed = 6.0f;
    private float            moveAcc = 9.0f;

    private float            moveHorizontal;
    private float            moveVertical;
    private float            walkVectical;


    private Vector3          standPoint;
    private Vector3          forwardDirection;

    [HideInInspector]
    public bool              wDown;

    // ���� ���� ����
    private float            jumpCooldown = 0;
    private float            maxJumpCooldown = 0.6f;

    // ȸ�� ���� ����
    private Vector3          torque;
    private float            rotateTorque = 45.0f;
    private float            maxAngularVelocity = 3.0f;

    // ������ ���� üũ�� ����
    private Ray              ray;
    private Vector3          rayOrigin;
    private float            maxDistance = 2;
    
    private int              layerToIgnore = 0;
    private int              layerMask = 0;

    // ROS���� ���ӵ��� �ӵ��� �޴� ��ũ��Ʈ
    [Header("Data Subscriber")]
    [SerializeField]
    private CmdVelSubscriber cmdSub;

    // ROS���� �޾ƿ� ������ ���� �����ӿ� ����
    // �̵� ���� ����
    private Vector3          linearVelocity;
    private Vector3          angularVelocity;

    // �ܺ� ������ 
    public float velocity; // UI������Ʈ�� ����
    public float AnimationMovement => animationMovement;
    public float MoveVertical => moveVertical;

    [HideInInspector]
    public bool              moveBack;

    void Awake()
    {
        playerRigidbody     = GetComponent<Rigidbody>();
        velocity            = 0.0f;
        state               = PlayerState.Idle;
        standPoint          = new Vector3();
        forwardDirection    = new Vector3();

        ray                 = new Ray();
        rayOrigin           = new Vector3();

        layerToIgnore       = LayerMask.GetMask("Player", "PlayerCollider", "Sensor");
        layerMask           = ~layerToIgnore;
    }
    
    // ����Ƽ ��ü��
    void UpdateInput()
    {
        // �÷��̾ ���鿡�� ������������ ���� ���ϰ�
        if (state == PlayerState.Jump) return;
        // ����Ƽ ��ü���� �����̱� ���� �޾ƿ��� Input
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        wDown = Input.GetButton("Walk");
        walkVectical = Input.GetAxis("Walk");
    }

    //  ���濡 ��簡 �ִ��� üũ + ���溤�� �����
    void CheckFrontSlope()
    {
        ray.direction   = Vector3.down;
        // ������ �̵��� ���� ������ �ڷ� �̵��� ���� �Ĺ��� üũ
        if(moveVertical >= 0)
            rayOrigin   = transform.position + playerRigidbody.transform.forward.normalized * 0.1f;
        else
            rayOrigin   = transform.position - playerRigidbody.transform.forward.normalized * 0.1f;
        rayOrigin.y     += maxDistance / 2;
        
        ray.origin      = rayOrigin;

        RaycastHit hit;

        // Raycast�ؼ� ���� ��ġ�� �������� ���� �� ���� ����, ��ġ�� ������ ���� �Ǵ� �Ĺ�����
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            forwardDirection = (hit.point - standPoint).normalized;
        else
            forwardDirection = playerRigidbody.transform.forward;
    }

    // �̵��ϱ� ���� ���� �ִ� �Լ�
    void UpdateMoveForce()
    {
        // �ȱ����� �޸��������� ���� �ִ� �ӵ� ����
        float targetSpeed = !wDown ? walkSpeed : runSpeed;

        Vector3 targetVelocity = movement.normalized * targetSpeed;
        Vector3 velocityDifference = targetVelocity - playerRigidbody.velocity;
        
        // �ӵ��� ���� �ε巯�� ���ӵ� ����
        Vector3 acceleration = velocityDifference.normalized * 
            Mathf.Lerp(moveAcc * 0.55f, moveAcc, ClampT(velocityDifference.magnitude / targetSpeed));
        
        if (Input.GetButton("Vertical"))
        {
            // �ִ� �ӵ� ���� �� ���ӵ� �߰�
            if (playerRigidbody.velocity.magnitude < targetSpeed)
                playerRigidbody.AddForce(acceleration, ForceMode.Acceleration);
        }
        
    }

    // ȸ���ϱ� ���� ���� �ִ� �Լ�
    void UpdateTorque()
    {
        torque.y = moveHorizontal;
        Vector3 targetAngularVelocity = torque.normalized * maxAngularVelocity;

        // ���� ȸ�� �ӵ��� ��ǥ ȸ�� �ӵ��� ���̸� ���
        Vector3 angularVelocityDifference = targetAngularVelocity - playerRigidbody.angularVelocity;

        // �ӵ��� ���� �ε巯�� �����ӵ� ����
        Vector3 angularAcceleration = angularVelocityDifference.normalized
            * Mathf.Lerp(0, rotateTorque, angularVelocityDifference.magnitude / maxAngularVelocity);

        // ĳ���Ͱ� ��ư���� �ն��� �ٷ� ȸ���� ���߰�
        if (Input.GetButton("Horizontal"))
        {
            if (playerRigidbody.angularVelocity.magnitude < maxAngularVelocity)
                playerRigidbody.AddTorque(angularAcceleration, ForceMode.Acceleration);

        }
    }

    // �̵� �ӵ��� ȸ�� ���� ������Ʈ �Լ�
    void UpdateMovement()
    {
        // �÷��̾ ���鿡�� ������������ ���� ���ϰ�
        if (state == PlayerState.Jump) return;

        float moveParam = 0.0f;

        if (moveVertical >= 0)
        {
            moveParam = (moveVertical + walkVectical) * 0.5f;
            if (moveVertical == 0)
                moveParam = 0;
            movement = forwardDirection * moveParam;
            animationMovement = playerRigidbody.velocity.magnitude / runSpeed;
        }
        else if(moveVertical < 0)
        {
            moveParam = (moveVertical - walkVectical) * 0.5f;
            movement = -forwardDirection * moveParam;
            animationMovement = - playerRigidbody.velocity.magnitude / runSpeed;
        }

        CheckFrontSlope();
        UpdateMoveForce();
        UpdateTorque();

    }

    // ���� �Լ�
    void UpdateJump()
    {
        // ��ٿ� ���ҿ� ���� ��Ȱ��ȭ
        if (jumpCooldown > 0)
            jumpCooldown -= Time.fixedDeltaTime;
        if (jumpCooldown > 0 || state == PlayerState.Jump) return;

        // �����̽��ٸ� ������ ����
        if(Input.GetButton("Jump"))
        {
            jumpCooldown = maxJumpCooldown;
            playerRigidbody.AddForce(Physics.gravity * (-0.7f) * playerRigidbody.mass, ForceMode.Impulse);
        }
    }

    // ROS���� �޾ƿ°ɷ� �����̴� �κ�
    // ROS �ӵ� ������ �޴� �κ�
    void UpdateRosData()
    {
        if (cmdSub == null) return;
        linearVelocity = cmdSub.LinearVelocity;
        angularVelocity = cmdSub.AngularVelocity;
    }

    // �޾ƿ� �����ͷ� �����̴� �κ�
    void UpdateROSMovement()
    {
        float moveParam = 0.0f;

        // ROS ��Ʋ���� �ְ� �ӵ��� 0.22�̱� ������ 100 / 22 �� ���ؼ� ����� �ӵ� �����.
        moveParam = linearVelocity.z * 100.0f / 22.0f; 

        if (moveParam < 0)
            moveBack = true;
        else 
            moveBack = false;

        movement = playerRigidbody.transform.forward * moveParam;
        playerRigidbody.velocity = movement * 5.0f;
        playerRigidbody.angularVelocity = angularVelocity;
    }

    void FixedUpdate()
    {
        UpdateInput();
        UpdateMovement();
        UpdateJump();
        
        // UpdateROSMovement();        

        velocity = playerRigidbody.velocity.magnitude;
    }

    float ClampT(float t)
    {
        if (t < 0) return 0;
        else if (t > 1) return 1;
        else return t;
    }
    
    public void SetState(PlayerState state)
    {
        this.state = state;
    }

    public void GetRayPoint(Vector3 point)
    {
        standPoint = point;
    }
}
