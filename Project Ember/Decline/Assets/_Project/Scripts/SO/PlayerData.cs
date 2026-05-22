using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _dashSpeed = 10f;
    public float MaxHealth => _maxHealth;
    public float MoveSpeed => _moveSpeed;
    public float DashSpeed => _dashSpeed;
}
