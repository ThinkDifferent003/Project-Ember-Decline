using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyBehavior {AGGRESSIVE , DEFENSIVE , CAUTIOUS}
[CreateAssetMenu(fileName = "NewEnemy", menuName = "EnemyData",order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string _enemyName;
    [SerializeField] private EnemyBehavior _personality;
    [Header("Health & Resistance")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _fireDamageMultiplier = 1;
    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackCooldown = 1f;
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 3;
    [SerializeField] private float _visualRange = 10f;
    [Header("Dash Settings")] 
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 2f;
    [Header("Stamina Settings")]
    [SerializeField] private float _staminaCostRate = 20f;
    [SerializeField] private float _staminaRegenRate = 10f;
    [Header("Reward Settings")]
    [SerializeField] private float _baseXpReward = 15f;

    #region - Public Proprierties -
    public string EnemyName => _enemyName;
    public float MaxHealth => _maxHealth;
    public float MoveSpeed => _moveSpeed;
    public float FireDamageMultiplier => _fireDamageMultiplier;
    public float VisualRange => _visualRange;
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;
    public EnemyBehavior Personality => _personality;
    public float DashSpeed => _dashSpeed;
    public float DashDuration => _dashDuration;
    public float DashCooldown => _dashCooldown;
    public float MaxStamina => _maxStamina;
    public float StaminaCostRate => _staminaCostRate;
    public float StaminaRegenRate => _staminaRegenRate;
    public float BaseXpReward => _baseXpReward;
    #endregion
}
