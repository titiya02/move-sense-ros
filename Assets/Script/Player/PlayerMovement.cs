using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody        playerRigidbody;

    // 유니티 내부적인 움직임용 변수들
    // 이동 관련 변수
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

    // 점프 관련 변수
    private float            jumpCooldown = 0;
    private float            maxJumpCooldown = 0.6f;

    // 회전 관련 변수
    private Vector3          torque;
    private float            rotateTorque = 45.0f;
    private float            maxAngularVelocity = 3.0f;

    // 전방의 경사면 체크용 변수
    private Ray              ray;
    private Vector3          rayOrigin;
    private float            maxDistance = 2;
    
    private int              layerToIgnore = 0;
    private int              layerMask = 0;

    // ROS에서 각속도와 속도를 받는 스크립트
    [Header("Data Subscriber")]
    [SerializeField]
    private CmdVelSubscriber cmdSub;

    // ROS에서 받아온 변수로 만들 움직임용 변수
    // 이동 관련 변수
    private Vector3          linearVelocity;
    private Vector3          angularVelocity;

    // 외부 참조용 
    public float velocity; // UI업데이트용 변수
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
    
    // 유니티 자체용
    void UpdateInput()
    {
        // 플레이어가 지면에서 떨어져있으면 조작 못하게
        if (state == PlayerState.Jump) return;
        // 유니티 자체에서 움직이기 위해 받아오는 Input
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        wDown = Input.GetButton("Walk");
        walkVectical = Input.GetAxis("Walk");
    }

    //  전방에 경사가 있는지 체크 + 전방벡터 추출용
    void CheckFrontSlope()
    {
        ray.direction   = Vector3.down;
        // 앞으로 이동할 때는 전방을 뒤로 이동할 때는 후방을 체크
        if(moveVertical >= 0)
            rayOrigin   = transform.position + playerRigidbody.transform.forward.normalized * 0.1f;
        else
            rayOrigin   = transform.position - playerRigidbody.transform.forward.normalized * 0.1f;
        rayOrigin.y     += maxDistance / 2;
        
        ray.origin      = rayOrigin;

        RaycastHit hit;

        // Raycast해서 나온 위치를 기준으로 힘을 줄 방향 지정, 위치가 없으면 전방 또는 후방으로
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            forwardDirection = (hit.point - standPoint).normalized;
        else
            forwardDirection = playerRigidbody.transform.forward;
    }

    // 이동하기 위한 힘을 주는 함수
    void UpdateMoveForce()
    {
        // 걷기인지 달리기인지에 따라 최대 속도 조정
        float targetSpeed = !wDown ? walkSpeed : runSpeed;

        Vector3 targetVelocity = movement.normalized * targetSpeed;
        Vector3 velocityDifference = targetVelocity - playerRigidbody.velocity;
        
        // 속도에 따른 부드러운 가속도 적용
        Vector3 acceleration = velocityDifference.normalized * 
            Mathf.Lerp(moveAcc * 0.55f, moveAcc, ClampT(velocityDifference.magnitude / targetSpeed));
        
        if (Input.GetButton("Vertical"))
        {
            // 최대 속도 제한 및 가속도 추가
            if (playerRigidbody.velocity.magnitude < targetSpeed)
                playerRigidbody.AddForce(acceleration, ForceMode.Acceleration);
        }
        
    }

    // 회전하기 위한 힘을 주는 함수
    void UpdateTorque()
    {
        torque.y = moveHorizontal;
        Vector3 targetAngularVelocity = torque.normalized * maxAngularVelocity;

        // 현재 회전 속도와 목표 회전 속도의 차이를 계산
        Vector3 angularVelocityDifference = targetAngularVelocity - playerRigidbody.angularVelocity;

        // 속도에 따른 부드러운 각가속도 설정
        Vector3 angularAcceleration = angularVelocityDifference.normalized
            * Mathf.Lerp(0, rotateTorque, angularVelocityDifference.magnitude / maxAngularVelocity);

        // 캐릭터가 버튼에서 손때면 바로 회전을 멈추게
        if (Input.GetButton("Horizontal"))
        {
            if (playerRigidbody.angularVelocity.magnitude < maxAngularVelocity)
                playerRigidbody.AddTorque(angularAcceleration, ForceMode.Acceleration);

        }
    }

    // 이동 속도와 회전 관련 업데이트 함수
    void UpdateMovement()
    {
        // 플레이어가 지면에서 떨어져있으면 조작 못하게
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

    // 점프 함수
    void UpdateJump()
    {
        // 쿨다운 감소와 점프 비활성화
        if (jumpCooldown > 0)
            jumpCooldown -= Time.fixedDeltaTime;
        if (jumpCooldown > 0 || state == PlayerState.Jump) return;

        // 스페이스바를 누르면 점프
        if(Input.GetButton("Jump"))
        {
            jumpCooldown = maxJumpCooldown;
            playerRigidbody.AddForce(Physics.gravity * (-0.7f) * playerRigidbody.mass, ForceMode.Impulse);
        }
    }

    // ROS에서 받아온걸로 움직이는 부분
    // ROS 속도 데이터 받는 부분
    void UpdateRosData()
    {
        if (cmdSub == null) return;
        linearVelocity = cmdSub.LinearVelocity;
        angularVelocity = cmdSub.AngularVelocity;
    }

    // 받아온 데이터로 움직이는 부분
    void UpdateROSMovement()
    {
        float moveParam = 0.0f;

        // ROS 터틀봇의 최고 속도가 0.22이기 때문에 100 / 22 를 곱해서 사람의 속도 맞춘다.
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
