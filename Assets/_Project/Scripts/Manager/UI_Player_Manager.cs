using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Player_Manager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image _healthBarFill;
    [SerializeField] private Image _energyBarFill;

    private PlayerData _playerData;
    private PlayerHealth _playerHealth;


    #region - Lyfe Cycle -
    private void Start()
    {
        FindPlayerReferences();
    }
    private void Update()
    {
        UpdateHealthBar();
        UpdateEnergybar();    
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
    }
    #endregion
}
