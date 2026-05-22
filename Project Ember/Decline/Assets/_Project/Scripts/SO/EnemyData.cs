using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "EnemyData",order = 1)]
public class EnemyData : ScriptableObject
{
    [SerializeField] private string _enemyName;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _moveSpeed = 3;
    [SerializeField] private float _fireDamageMultiplier = 1;
    [SerializeField] private float _visualRange = 1f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackCooldown = 1f;

    public string EnemyName => _enemyName;
    public float MaxHealth => _maxHealth;
    public float MoveSpeed => _moveSpeed;
    public float FireDamageMultiplier => _fireDamageMultiplier;
    public float VisualRange => _visualRange;
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;
}
