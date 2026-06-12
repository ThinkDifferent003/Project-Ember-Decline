using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    private PlayerLeveling _playerLeveling;
    private PlayerSetup _playerSetup;
    private float _currentStamina;
    private float _maxStamina;
    private float _regenRate;
    private bool _isRegenBlocked = false;
    public float CurrentStamina => _currentStamina;
    public float MaxStamina => _maxStamina;

    private void Start()
    {
        _playerLeveling = GetComponent<PlayerLeveling>();
        _playerSetup = GetComponent<PlayerSetup>();
        UpdateStaminaStats();
        _currentStamina = _maxStamina;
    }
    private void Update()
    {
        if (_isRegenBlocked) return;
        if (_currentStamina < _maxStamina)
        {
            _currentStamina += _regenRate * Time.deltaTime;
            _currentStamina = Mathf.Min(_currentStamina, _maxStamina);
        }
    }
    public void UpdateStaminaStats()
    {
        if (_playerSetup != null && _playerSetup.PlayerData != null && _playerLeveling != null)
        {
            _maxStamina = _playerSetup.PlayerData.GetStamina(_playerLeveling.CurrentLevel);
            _regenRate = _playerSetup.PlayerData.StaminaRegenRate;
        }
    }
    public void SetRegenerationBlocked(bool blocked)
    {
        _isRegenBlocked = blocked;
    }
    public bool TryConsumeStamina(float amount)
    {
        if (_currentStamina >= amount)
        {
            _currentStamina -= amount;
            return true;
        }
        return false;
    }
}
