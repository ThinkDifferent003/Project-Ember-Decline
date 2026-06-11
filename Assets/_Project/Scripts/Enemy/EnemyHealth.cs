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
    private int _enemyLevel = 1;
    private float _enemyDefense = 0f;

    private float _currentHealth;  
    private bool _isDead; 
    private bool _isStunned;
    private float _dmgMultiplier = 1;
    #region - Public Proprierties -
    public float CurrentHealth => _currentHealth;
    public float SpeedMultiplier { get; private set; } = 1f;
    public bool IsDead => _isDead;
    public bool IsStunned => _isStunned;
    public int EnemyLevel => _enemyLevel;
    public float DamageMultiplier => _dmgMultiplier;
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
        var defensiveEnemy = GetComponent<EnemyDefensive>();
        Debug.Log($"[DEBUG DIFESA] Il nemico {gameObject.name} ha {_enemyDefense} di difesa. Riceve base: {damage}");
        float dmgAfterDef = damage - _enemyDefense;
        if (dmgAfterDef < 1f) dmgAfterDef = 1f;
        float finalDmg = dmgAfterDef;
        if (defensiveEnemy != null && defensiveEnemy.IsShieldActive)
        {
            finalDmg = dmgAfterDef * defensiveEnemy.ShieldDamageMultiplier;
            Debug.Log($"🛡️ Scudo ATTIVO! Danno post-difesa: {dmgAfterDef} -> Danno ridotto: {finalDmg}");
        }
        else Debug.Log($"💥 Scudo INATTIVO. Danno pieno: {finalDmg}");
        _currentHealth -= finalDmg;
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
        if (_enemyData != null)
        {
            float xp = _enemyData.BaseXpReward * (1f + (_enemyLevel - 1) * 0.5f);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerLeveling playerLeveling = player.GetComponent<PlayerLeveling>();
                if (playerLeveling != null)
                {
                    playerLeveling.AddXp(xp);
                    Debug.Log($"✨ Sconfitto {_enemyData.EnemyName} (Liv. {_enemyLevel}). Assegnati {xp} XP!");
                }
            }
        }
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
    public void Inizialize(EnemyData data , int level)
    {
        _enemyData = data;
        _enemyLevel = level;
        float maxHealthScale = _enemyData.GetMaxHealth(_enemyLevel);
        _currentHealth = maxHealthScale;
        _dmgMultiplier = _enemyData.GetDamageMultiplier(_enemyLevel);
        _enemyDefense = _enemyData.GetDefense(_enemyLevel);
        Debug.Log($"{gameObject.name} Inizializzato al Livello {_enemyLevel}. HP Massimi: {_currentHealth} Difesa: {_enemyDefense}");
    }
    #endregion
}
