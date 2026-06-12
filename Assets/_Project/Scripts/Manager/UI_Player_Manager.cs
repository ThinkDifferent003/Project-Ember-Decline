using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Manager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image _healthBarFill;
    [SerializeField] private Image _energyBarFill;
    [SerializeField] private Image _staminaBarFill;

    private PlayerData _playerData;
    private PlayerHealth _playerHealth;
    private PlayerEnergy _playerEnergy;
    private PlayerStamina _playerStamina;


    #region - Lyfe Cycle -
    private void Start()
    {
        FindPlayerReferences();
    }
    private void Update()
    {
        UpdateHealthBar();
        UpdateEnergybar();    
        UpdateStaminaBar();
    }
    #endregion
    #region - Core Logic -
    private void FindPlayerReferences()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerSetup setup = playerObj.GetComponent<PlayerSetup>();
            if (setup != null) _playerData = setup.PlayerData;
            _playerHealth = playerObj.GetComponent<PlayerHealth>();
            _playerEnergy = playerObj.GetComponent<PlayerEnergy>();
            _playerStamina = playerObj.GetComponent<PlayerStamina>();
        }
    }
    private void UpdateHealthBar()
    {
        if (_healthBarFill == null || _playerHealth == null || _playerData == null) return;
        float maxHealth = _playerHealth.MaxHealth;
        if (maxHealth > 0)
        {
            float healthRatio = _playerHealth.CurrentHealth / maxHealth;
            _healthBarFill.fillAmount = Mathf.Clamp01(healthRatio);
        }
       
    }
    private void UpdateEnergybar()
    {
        if (_energyBarFill == null || _playerData == null) return;
        float maxEnergy = _playerEnergy.MaxEnergy;
        if (maxEnergy > 0)
        {
            float energyRatio = _playerEnergy.CurrentEnergy / maxEnergy;
            _energyBarFill.fillAmount = Mathf.Clamp01(energyRatio);
        }
    }
    private void UpdateStaminaBar()
    {
        if (_playerStamina == null)
        {
            FindPlayerReferences();
        }
        if (_staminaBarFill == null) return;
        float maxStamina = _playerStamina.MaxStamina;
        if (maxStamina > 0)
        {
            float staminaRatio = _playerStamina.CurrentStamina / maxStamina;
            _staminaBarFill.fillAmount = Mathf.Clamp01(staminaRatio);
        }
    }
    #endregion
}
