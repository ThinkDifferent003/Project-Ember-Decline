using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggressive : EnemyAI
{
    private bool _isDashing;
    private float _lastDashTime;

    #region - Core Logic -
    protected override void PerformLogic()
    {
        if (_currentStamina <= 0 && !_isExhausted)
        {
            _currentStamina = 0;
            _isExhausted = true;
        }
        if (_isExhausted)
        {
            RegenerateStamina();
            if (_isExhausted) return;
        }
        if (_agent == null || !_agent.enabled) return;
        if (_isDashing) return;
        if (TryDash()) return;
        float dist = Vector3.Distance(transform.position, _playerTransform.position);
        HandleStandarBehavior(dist);
    }
    private bool TryDash()
    {
        if (_currentStamina < _enemyData.StaminaCostRate) return false;
        float dist = Vector3.Distance(transform.position, _playerTransform.position);
        if (dist < 2f) return false;
        float dashChance = 0.01f;
        if (dist > 5f) dashChance += 0.05f;
        if (GetPlayerHealth() < 0.4f) dashChance += 0.1f;
        if (Random.value < dashChance && Time.time > _lastDashTime + _enemyData.DashCooldown)
        {
            _currentStamina -= _enemyData.StaminaCostRate;
            StartCoroutine(DashRoutine());
            return true;
        }
        return false;
    }
    #endregion
    #region - Coroutine -
    private IEnumerator DashRoutine()
    {
        _isDashing = true;
        _lastDashTime = Time.time;
        _agent.enabled = false;
        Vector3 dashDir = (_playerTransform.position - transform.position).normalized;
        float timer = 0f;
        while (timer < _enemyData.DashDuration)
        {
            transform.position += dashDir * _enemyData.DashSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        _agent.enabled = true;
        _agent.ResetPath();
        _isDashing = false;
    }
    #endregion
}
