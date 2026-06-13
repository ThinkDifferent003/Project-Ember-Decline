using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDefensive : EnemyAI
{
    [Header("Shield Settings")]
    [SerializeField] private float _shieldDamageMultiplier = 0.5f;
    [SerializeField] private float _shieldCostMultiplier = 0.5f;
    [Header("Movement Speed Multipliers")]
    [SerializeField] private float _speedMultiplier = 0.5f;
    [SerializeField] private float _exhaustedSpeedMultiplier = 0.3f;
    [Header("Attack Timings")]
    [SerializeField] private float _attackDuration = 1.2f;
    [SerializeField] private float _minWaitBeforeAttack = 0.8f;
    [SerializeField] private float _maxWaitBeforeAttack = 2.0f;
    [Header("Strafe Timings")]
    [SerializeField] private float _minWaitBeforeStrafe = 1.5f;
    [SerializeField] private float _maxWaitBeforeStrafe = 3.5f;

    private bool _isShieldActive;
    private bool _isTelekinesisDisabled;
    private bool _isAttacking;
    private bool _shouldStrafe;

    private float _strafeTimer;
    private float _strafeDecisionTimer;
    private float _currentStrafeDir = 1f;
    private float _attackTimer;
    public bool IsShieldActive => _isShieldActive;
    public float ShieldDamageMultiplier => _shieldDamageMultiplier;

    #region Public Methods
    public void SetTelekinesisDisable(bool disable)
    {
        _isTelekinesisDisabled = disable;
        if (disable) ToggleShield(false);
    }
    public void ToggleShield(bool active)
    {
        if (_isShieldActive == active) return; 
        _isShieldActive = active;
        if (_enemyAnim != null) _enemyAnim.SetShieldState(_isShieldActive);
        if (_isShieldActive) Debug.Log($"🛡️ [{gameObject.name}] STATO DIFENSIVO ATTIVO: Scudo alzato, danni ridotti del {(1f - _shieldDamageMultiplier) * 100}%!");
        else Debug.Log($"❌ [{gameObject.name}] STATO DIFENSIVO DISATTIVATO: Scudo abbassato.");
    }
    #endregion
    #region Core Logic
    protected override void PerformLogic()
    {
        if (_weaponObj != null && !_weaponObj.activeSelf) _weaponObj.SetActive(true); 
        CheckStamina();
        if (_isAttacking)
        {
            _agent.isStopped = true;
            RotateTowardsPlayer();
            return;
        }
        float distToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
        if (_isExhausted)
        {
            HandleExhaustedState();
            if (_isExhausted) return;
        }
        if (_agent == null || !_agent.enabled || !_agent.isOnNavMesh) return;
        HandleShieldManagement();
        float combatRange = _enemyData != null ? _enemyData.AttackRange + 1.5f : 3.5f;
        if (distToPlayer <= combatRange) HandleCombatBehavior(distToPlayer);
        else HandleDistantBehavior(distToPlayer);
    }
    #endregion
    #region State Handlers
    private void CheckStamina()
    {
        if (_currentStamina <= 0 && !_isExhausted)
        {
            _currentStamina = 0;
            _isExhausted = true;
        }
    }
    private void HandleExhaustedState()
    {
        RegenerateStamina();
        ToggleShield(false);
        RotateTowardsPlayer();
        if (_agent != null && _agent.enabled && _agent.isOnNavMesh) StrafeMovement(true);
    }
    private void HandleShieldManagement()
    {
        if (!_isTelekinesisDisabled && _currentStamina > 0)
        {
            ToggleShield(true);
            _currentStamina -= (_enemyData.StaminaCostRate * _shieldCostMultiplier) * Time.deltaTime;
        }
        else ToggleShield(false);
    }
    private void HandleCombatBehavior(float distToPlayer)
    {
        RotateTowardsPlayer();
        _strafeDecisionTimer -= Time.deltaTime;
        if (_strafeDecisionTimer <= 0) ChooseMovementBehavior();
        if (_enemyData != null && distToPlayer <= _enemyData.AttackRange && _colldownTimer <= 0)
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0) StartCoroutine(ExecuteDefensiveAttacking());
            else HandleCloseRangeMovement();
        }
        else
        {
            HandleCloseRangeMovement();
        }
    }
    private void HandleDistantBehavior(float distToPlayer)
    {
        _agent.isStopped = false;
        if (_enemyData != null) _agent.speed = _enemyData.MoveSpeed * _speedMultiplier;
        HandleStandarBehavior(distToPlayer);
        if (_colldownTimer <= 0 && _attackTimer <= 0) ResetAttackTimer();
        if (_strafeDecisionTimer <= 0) ResetStrafeTimer();
    }
    #endregion
    #region Movement Decision & Execution
    private void HandleCloseRangeMovement()
    {
        if (_shouldStrafe) StrafeMovement(false);
        else _agent.isStopped = true;
    } 
    private void ChooseMovementBehavior()
    {
        _shouldStrafe = Random.value > 0.5f;
        ResetStrafeTimer();
    }
    private void StrafeMovement(bool isExhausted)
    {
        _strafeTimer -= Time.deltaTime;
        if (_strafeTimer <= 0)
        {
            _currentStrafeDir = Random.value > 0.5f ? 1f : -1f;
            _strafeTimer = Random.Range(1.5f, 3f);
        }
        Vector3 dirToPlayer = (_playerTransform.position - transform.position).normalized;
        Vector3 lateralDir = Vector3.Cross(dirToPlayer, Vector3.up) * _currentStrafeDir;
        Vector3 targetPos = transform.position + lateralDir * 1.5f + dirToPlayer * 0.5f;
        _agent.isStopped = false;
        float currentMultiplier = isExhausted ? _exhaustedSpeedMultiplier : _speedMultiplier;
        _agent.speed = _enemyData != null ? _enemyData.MoveSpeed * currentMultiplier : 2f;
        _agent.SetDestination(targetPos);
    }
    #endregion
    #region Timers Reset
    private void ResetStrafeTimer()
    {
        _strafeDecisionTimer = Random.Range(_minWaitBeforeStrafe, _maxWaitBeforeStrafe);
    }
    private void ResetAttackTimer()
    {
        _attackTimer = Random.Range(_minWaitBeforeAttack,_maxWaitBeforeAttack);
    }
    #endregion
    #region Coroutine
    private IEnumerator ExecuteDefensiveAttacking()
    {
        _isAttacking = true;
        _agent.isStopped = true;
        ToggleShield(false);
        Attack();
        yield return new WaitForSeconds(_attackDuration);
        ResetAttackTimer();
        ChooseMovementBehavior();
        _isAttacking = false;
    }
    #endregion
}
