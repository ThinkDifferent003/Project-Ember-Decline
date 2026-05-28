using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowersManager : MonoBehaviour
{
    [Header("Components")]
    private ProstheticPower[] _prostheticPowers;
    private PlayerData _playerData;
    private PlayerHealth _playerHealth;

    private int _currentPowerIndex = 0;
    private bool _canActivated = true;

    #region - Lyfe Cycle -
    private void Start()
    {
        _prostheticPowers = GetComponentsInChildren<ProstheticPower>(true);
        _playerHealth = GetComponent<PlayerHealth>();
        PlayerSetup setup = GetComponent<PlayerSetup>();
        if (setup != null) _playerData = setup.PlayerData;
    }
    private void Update()
    {
        if (_prostheticPowers == null || _prostheticPowers.Length == 0) return; 
        if (_playerHealth != null && _playerHealth.IsStunned)
        {
            HandleStunState();
            return;
        }
        HandleInput();
    }
    #endregion
    #region - Core Logic -
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) SwitchPower();
        if (Input.GetKey(KeyCode.Q))
        {
            if (_canActivated)
            {
                ActivateSelectedPower();
                if (!_prostheticPowers[_currentPowerIndex].IsActive) _canActivated = false;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            DeactivateSelectedPower();
            _canActivated = true;
        }
    }
    private void HandleStunState()
    {
        if (_prostheticPowers[_currentPowerIndex].IsActive) DeactivateSelectedPower();
        if (Input.GetKey(KeyCode.Q)) _canActivated = false;
    }
    private void SwitchPower()
    {
        if (_prostheticPowers[_currentPowerIndex].IsActive) return;
        _currentPowerIndex++;
        if (_currentPowerIndex >= _prostheticPowers.Length) _currentPowerIndex = 0;
    }
    #endregion
    #region - Select & Control Powers -
    private void ActivateSelectedPower()
    {
        _prostheticPowers[_currentPowerIndex]?.Activate();
    }
    private void DeactivateSelectedPower()
    {
        _prostheticPowers[_currentPowerIndex]?.ForceDeactivate();
    }
    public ProstheticPower GetCurrentSelectedPower()
    {
        if (_prostheticPowers != null && _prostheticPowers.Length > 0) return _prostheticPowers[_currentPowerIndex];
        return null;
    }
    #endregion
}
