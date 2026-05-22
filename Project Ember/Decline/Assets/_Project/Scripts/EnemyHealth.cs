using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour , IBurnable
{
    private EnemyData _enemyData;
    private float _currentHealth; 
    private Rigidbody _rb; 
    private bool _isDead;
    private EnemyVisuals _enemyVisuals;

    private void Start()
    {  
        _rb = GetComponent<Rigidbody>();
        _enemyVisuals = GetComponent<EnemyVisuals>();   
    }
    public void Inizialize(EnemyData data)
    {
        _enemyData = data;
        _currentHealth = _enemyData.MaxHealth;
    }
    public void TakeDamage(float damage , Vector3 knockbackDir, float knockbackForce)
    {
        if (_isDead) return;
        _currentHealth -= damage;
        Debug.Log($"Colpito! Vita rimanente di {gameObject.name}: {_currentHealth}");
        if (_currentHealth <= 0)
        {
            Die();
            return;
        }
        if (_enemyVisuals != null) _enemyVisuals.FlashHit();
        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
        }
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
        if (_enemyVisuals != null) _enemyVisuals.FlashBurn();
    }
    
    private void Die()
    {
        _isDead = true;
        Destroy(gameObject);
    }
}
