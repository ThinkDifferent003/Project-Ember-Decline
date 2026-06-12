using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    private PlayerLeveling _playerLeveling;
    private PlayerSetup _playerSetup;
    private float _currentEnergy;
    private float _maxEnergy;
    private float _regenRate;
    private bool _isRegenBlocked = false;
    public float CurrentEnergy => _currentEnergy;
    public float MaxEnergy => _maxEnergy;

    private void Start()
    {
        _playerLeveling = GetComponent<PlayerLeveling>();
        _playerSetup = GetComponent<PlayerSetup>();
        UpdateEnergyStats();
        _currentEnergy = _maxEnergy;
    }
    private void Update()
    {
        if (_isRegenBlocked) return;
        if (_currentEnergy < _maxEnergy)
        {
            _currentEnergy += _regenRate * Time.deltaTime;
            _currentEnergy = Mathf.Min(_currentEnergy, _maxEnergy);
        }
    }
    public void SetRegenerationBlocked(bool blocked)
    {
        _isRegenBlocked = blocked;
    }
    public void UpdateEnergyStats()
    {
        if (_playerSetup != null && _playerSetup.PlayerData != null && _playerLeveling != null)
        {
            _maxEnergy = _playerSetup.PlayerData.GetMaxEnergy(_playerLeveling.CurrentLevel);
            _regenRate = _playerSetup.PlayerData.EneryRegenRate;
        }
    }
    public bool TryConsumeEnergy(float amount)
    {
        if (_currentEnergy >= amount)
        {
            _currentEnergy -= amount;
            return true;
        }
        return false;
    }
}
