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
        if (_isDashing || _agent == null || !_agent.enabled) return;
        if (TryDash()) return;
        float dist = Vector3.Distance(transform.position, _playerTransform.position);
        HandleStandarBehavior(dist);
    }
    private bool TryDash()
    {
        float dist = Vector3.Distance(transform.position, _playerTransform.position);
        if (dist < 2f) return false;
        float dashChance = 0.025f;
        if (dist > 5f) dashChance += 0.1f;
        if (GetPlayerHealth() < 0.4f) dashChance += 0.15f;
        if (Random.value < dashChance && Time.time > _lastDashTime + _enemyData.DashCooldown)
        {
            StartCoroutine(DashRoutine());
            return true;
        }
        return false;
    }
    private void HandleStandarBehavior(float dist)
    {
        SetEquippedState(dist <= _enemyData.VisualRange);
        if (dist <= _enemyData.AttackRange)
        {
            _agent.isStopped = true;
            if (_colldownTimer <= 0) Attack();
            RotateTowardsPlayer();
        }
        else if (dist <= _enemyData.VisualRange)
        {
            _agent.SetDestination(_playerTransform.position);
            _agent.isStopped = false;
        }
        else _agent.isStopped = true;
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
