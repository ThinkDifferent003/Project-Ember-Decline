using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerData _playerData;
    private float _currentHealth;
    private PlayerAnimation _playerAnim;
    private bool _isDead = false;
    private bool _isStunned = false;
    public bool IsStunned => _isStunned;

    private void Start()
    {
        _playerAnim = GetComponent<PlayerAnimation>();
    }
    public void Inizialize(PlayerData data)
    {
        _playerData = data;
        _currentHealth = _playerData.MaxHealth;
    }
    public void TakeDamage(float damage)
    {
        if (_isDead) return;
        _currentHealth -= damage;
        float maxHealth;
        if (_playerData != null) maxHealth = _playerData.MaxHealth;
        if (_currentHealth <= 0) Die();
        else
        {
            if (_playerAnim != null) _playerAnim.TriggerHurt();
            StartCoroutine(StunRoutine());
        }
    }
    private void Die()
    {
        _isDead = true;
    }
    private IEnumerator StunRoutine()
    {
        _isStunned = true;
        yield return new WaitForSeconds(0.5f);
        _isStunned = false;
    }
}
