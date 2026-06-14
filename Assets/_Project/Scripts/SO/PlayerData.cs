using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private string _pgName;
    [Header("Health Settings")]
    [SerializeField] private AnimationCurve _healthCurve;
    [SerializeField] private AnimationCurve _defenseCurve;
    [Header("Stamina Settings")]
    [SerializeField] private AnimationCurve _staminaCurve;
    [SerializeField] private float _staminaRegenRate = 5f;
    [Header("Movement Setiings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _dashSpeed = 10f;
    [Header("Energy Settings")]
    [SerializeField] private AnimationCurve _energyCurve;
    [SerializeField] private float _energyRegenRate = 5f;
    [SerializeField] private AnimationCurve _attackMultiplierCurve;

    #region - Public Proprierties -
    public string PgName => _pgName;
    public float GetMaxHealth(int level) => _healthCurve.Evaluate(level);
    public float MoveSpeed => _moveSpeed;
    public float DashSpeed => _dashSpeed;
    public float GetMaxEnergy(int level) => _energyCurve.Evaluate(level);
    public float EneryRegenRate => _energyRegenRate;
    public float GetAttackMultiplier(int level) => _attackMultiplierCurve.Evaluate(level);
    public float GetDefense(int level) => _defenseCurve.Evaluate(level);
    public float GetStamina(int level) => _staminaCurve.Evaluate(level);
    public float StaminaRegenRate => _staminaRegenRate;
    #endregion
}
