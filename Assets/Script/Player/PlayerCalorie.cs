using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCalorie : MonoBehaviour
{
    [SerializeField]
    private PlayerState playerState;
    private Rigidbody   playerRigidBody;

    private float       usedCalorie;
    
    public float        UsedCalorie => usedCalorie;
    
    void Awake()
    {
        playerState = PlayerState.Idle;
        playerRigidBody = GetComponent<Rigidbody>();
        usedCalorie = 0.0f;
    }

    public void SetState(PlayerState state)
    {
        playerState = state;
    }

    // 상태에 따라 칼로리 소모 축적
    void UpdateCalorie()
    {
        float MET = 0.0f;
        float weight = playerRigidBody.mass;
        switch(playerState)
        {
            case PlayerState.Idle:
                MET = 1.0f;
                break;
            case PlayerState.Walk:
                MET = 3.4f;
                break;
            case PlayerState.Run:
                MET = 4.8f;
                break;
            case PlayerState.Jump:
                break;
            default:
                break;
        }

        usedCalorie = MET * weight / 3600 * Time.deltaTime;
    }

    void Update()
    {
        UpdateCalorie();
    }
}
