using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Health Settings")]
    [SerializeField] private AnimationCurve _healthCurve;
    [Header("Movement Setiings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _dashSpeed = 10f;
    [Header("Energy Settings")]
    [SerializeField] private AnimationCurve _energyCurve;
    [SerializeField] private float _energyRegenRate = 5f;

    #region - Public Proprierties -
    public float GetMaxHealth(int level) => _healthCurve.Evaluate(level);
    public float MoveSpeed => _moveSpeed;
    public float DashSpeed => _dashSpeed;
    public float GetMaxEnergy(int level) => _energyCurve.Evaluate(level);
    public float EneryRegenRate => _energyRegenRate;
    #endregion
}
