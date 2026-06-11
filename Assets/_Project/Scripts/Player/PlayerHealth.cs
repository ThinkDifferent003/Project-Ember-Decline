using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Components")]
    private PlayerData _playerData;
    private PlayerAnimation _playerAnim;

    private float _currentHealth;
    private bool _isDead = false;
    private bool _isStunned = false;
    private int _currentLevel = 1;
    #region - Puplic Proprierties -
    public float CurrentHealth => _currentHealth;
    public bool IsStunned => _isStunned;
    public float MaxHealth => _playerData.GetMaxHealth(_currentLevel);
    #endregion
    #region - Life Cycle -
    private void Start()
    {
        _playerAnim = GetComponent<PlayerAnimation>();
        if (_playerData == null)
        {
            PlayerSetup setup = GetComponent<PlayerSetup>();
            if (setup != null) Inizialize(setup.PlayerData);
        }
    }
    #endregion
    #region - Core Logic -
    public void TakeDamage(float damage)
    {
        if (_isDead) return;
        _currentHealth -= damage;
        Debug.Log("Salute attuale dopo il danno: " + _currentHealth);
        if (_currentHealth <= 0) Die();
        else HurtEffects();
    }
    private void HurtEffects()
    {
        if (_playerAnim != null) _playerAnim.TriggerHurt();
        StopAllCoroutines();
        StartCoroutine(StunRoutine());
    }
    private void Die()
    {
        _isDead = true;
    }
    #endregion
    #region - Coroutine -
    private IEnumerator StunRoutine()
    {
        _isStunned = true;
        yield return new WaitForSeconds(0.5f);
        _isStunned = false;
    }
    #endregion
    #region - Utility -
    public void Inizialize(PlayerData data)
    {
        _playerData = data;
        _currentHealth = MaxHealth;
    }
    public void UpdateHealthOnLevelUp(int newLevel)
    {
        _currentLevel = newLevel;
        _currentHealth = MaxHealth;
        Debug.Log($"PlayerHealth aggiornato al livello {_currentLevel}. Nuovi HP Massimi: {MaxHealth}");
    }
    #endregion
}
