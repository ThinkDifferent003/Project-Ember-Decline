using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDefensive : EnemyAI
{
    [SerializeField] private float _shieldDamageMultiplier = 0.5f;
    private bool _isShieldActive;
    private bool _isTelekinesisDisabled;
    public bool IsShieldActive => _isShieldActive;
    public float ShieldDamageMultiplier => _shieldDamageMultiplier;

    public void SetTelekinesisDisable(bool disable)
    {
        _isTelekinesisDisabled = disable;
        if (disable) ToggleShield(false);
    }
    public void ToggleShield(bool active)
    {
        _isShieldActive = active;
    }
    protected override void PerformLogic()
    {
        
        if (_isExhausted)
        {
            RegenerateStamina();
            if (_agent != null && _agent.enabled) _agent.isStopped = false;
        }
        else
        {
            if (!_isTelekinesisDisabled && _currentStamina > 0)
            {
                if (!_isShieldActive) // Logga solo quando cambia stato
                {
                    Debug.Log("??? Scudo attivato!");
                }
                ToggleShield(true);
                _currentStamina -= (_enemyData.StaminaCostRate * 2) * Time.deltaTime;
            }
            else
            {
                if (_isShieldActive) // Logga solo quando cambia stato
                {
                    Debug.Log("??? Scudo disattivato.");
                }
                ToggleShield(false);
            }
        }
        float dist = Vector3.Distance(transform.position, _playerTransform.position);
        HandleStandarBehavior(dist);
        if (!_isExhausted && _agent != null && _agent.enabled)
        {
            _agent.isStopped = false;
        }
    }
}
