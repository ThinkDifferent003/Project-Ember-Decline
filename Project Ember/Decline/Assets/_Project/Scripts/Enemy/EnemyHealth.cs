using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour , IBurnable
{
    private EnemyData _enemyData;
    private Rigidbody _rb;
    private EnemyVisuals _enemyVisuals;
    private Coroutine _slowCoroutine;
    private Coroutine _stunCoroutine;

    private float _currentHealth;  
    private bool _isDead; 
    private bool _isStunned;
    #region - Public Proprierties -
    public float CurrentHealth => _currentHealth;
    public float SpeedMultiplier { get; private set; } = 1f;
    public bool IsStunned => _isStunned;
    #endregion

    #region - Lyfe Cycle -
    private void Start()
    {  
        _rb = GetComponent<Rigidbody>();
        _enemyVisuals = GetComponent<EnemyVisuals>();   
    }
    #endregion
    #region - Core Logic -
    public void TakeDamage(float damage , Vector3 knockbackDir, float knockbackForce ,float stunDur = 0.5f)
    {
        if (_isDead) return;
        _currentHealth -= damage;
        Debug.Log($"Colpito! Vita rimanente di {gameObject.name}: {_currentHealth}");
        if (_currentHealth <= 0)
        {
            Die();
            return;
        }
        _enemyVisuals?.FlashHit();
        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
        }
        ApplyStun(stunDur);
    }
    public void Burn(float damage)
    {
        if (_isDead) return;
        float multiplier = 1f;
        if (_enemyData != null) multiplier = _enemyData.FireDamageMultiplier;
        float finalDamage = damage * multiplier;
        _currentHealth -= finalDamage;
        Debug.Log($"Danno: {finalDamage:F1} | Vita Rimasta: {Mathf.Max(0f, _currentHealth):F1}");
        if (_currentHealth <= 0)
        {
            Die();
            return;
        }
        _enemyVisuals?.FlashBurn();
        if (!_isStunned) ApplyStun(0.2f);
    }
    private void Die()
    {
        _isDead = true;
        Destroy(gameObject);
    }
    public void ApplySlow (float multiplier,float duration)
    {
        if (_slowCoroutine != null) StopCoroutine(_slowCoroutine);
        _slowCoroutine = StartCoroutine(SlowRoutine(multiplier,duration));
    }
    private void ApplyStun(float duration)
    {
        if (_stunCoroutine != null) StopCoroutine(_stunCoroutine);
        _stunCoroutine = StartCoroutine(StunRoutine(duration));
    }
    #endregion
    #region - Effetc State -
    private IEnumerator SlowRoutine(float multiplier, float duration)
    {
        SpeedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        SpeedMultiplier = 1f;
    }
    private IEnumerator StunRoutine(float duration)
    {
        _isStunned = true;
        yield return new WaitForSeconds(duration);
        _isStunned = false;
    }
    #endregion
    #region - Utility -
    public void Inizialize(EnemyData data)
    {
        _enemyData = data;
        _currentHealth = _enemyData.MaxHealth;
    }
    #endregion
}
