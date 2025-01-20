using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField]
    private GameObject unityPanel;
    [SerializeField]
    private GameObject rosPanel;

    [Header("Player")]
    [SerializeField]
    private GameObject player;
    
    // 플레이어 하위 스크립트
    private PlayerMovement playerMovement;
    private ImuSensor imuSensor;
    private PlayerCalorie playerCalorie;

    [Header("PlayerUI")]
    [SerializeField]
    private TextMeshProUGUI playerPosText;
    [SerializeField]
    private TextMeshProUGUI playerRotText;
    [SerializeField]
    private TextMeshProUGUI imuAngText;
    [SerializeField]
    private TextMeshProUGUI imuAccText;
    [SerializeField]
    private TextMeshProUGUI playerVelocityText;
    [SerializeField]
    private TextMeshProUGUI playerCalorieText;

    [Header("RosDataUI")]
    [SerializeField]
    private TextMeshProUGUI cmdLinear;
    [SerializeField]
    private TextMeshProUGUI cmdVelocity;

    private void Awake()
    {
        if(rosPanel != null) rosPanel.SetActive(false);
        if(player != null )
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            imuSensor = player.GetComponentInChildren<ImuSensor>();
            playerCalorie = player.GetComponent<PlayerCalorie>();
        }
    }

    // 플레이어 UI 업데이트
    void PlayerTextUpdate()
    {
        if(playerPosText != null)
            playerPosText.text = $"PlayerPos : {player.transform.position.z:F2}, {-player.transform.position.x:F2}, {player.transform.position.y:F2}";
        if (playerRotText != null)
            playerRotText.text = $"PlayerRot : {-player.transform.rotation.z:F2}, {player.transform.rotation.x:F2}, {-player.transform.rotation.y:F2}, {player.transform.rotation.w:F2}";
        if (imuAngText != null)
            imuAngText.text = $"ImuAng : {imuSensor.angularVelocity.x:F2}, {imuSensor.angularVelocity.y:F2}, {imuSensor.angularVelocity.z:F2}";
        if (imuAccText != null)
            imuAccText.text = $"ImuAcc : {imuSensor.linearAcceleration.x:F2}, {imuSensor.linearAcceleration.y:F2}, {imuSensor.linearAcceleration.z:F2}";
        if (playerVelocityText != null)
            playerVelocityText.text = $"Velocity : {playerMovement.velocity:F2}";
        if (playerCalorieText != null)
            playerCalorieText.text = $"Calorie : {playerCalorie.UsedCalorie:F4}";
    }
    
    void LateUpdate()
    {
        PlayerTextUpdate();
    }
}