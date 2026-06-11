using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    private float _damage;
    private float _knockbackForce;
    private bool _canDealDamage = false;

    private PlayerLeveling _playerLeveling;
    private Collider _weapon;
    private TrailRenderer _trail;

    #region - Lyfe Cycle -
    private void Start()
    {
        _weapon = GetComponent<Collider>();
        _trail = GetComponentInChildren<TrailRenderer>();
        _playerLeveling = transform.root.GetComponent<PlayerLeveling>();
        EnableDamage(0);
    }
    #endregion
    #region - Core Logic -
    public void InizializeWeaponData(float damage , float knockback)
    {
        _damage = damage;
        _knockbackForce = knockback;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_canDealDamage) return;
        if (other.gameObject == transform.root.gameObject) return;
        Vector3 hitDir = (other.transform.position - transform.position).normalized;
        hitDir.y = 0;
        if (hitDir == Vector3.zero) hitDir = transform.forward;
        TryDamageEnemy(other,hitDir);
        TryDamagePlayer(other);
    }
    private void TryDamageEnemy(Collider other, Vector3 hitDir)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>() ?? other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            float finalDmg = _damage;
            if (_playerLeveling != null)
            {
                finalDmg = _damage * _playerLeveling.AttackMultiplier;
                Debug.Log($"⚔️ Player attacca! Danno Base: {_damage} x Moltiplicatore ({_playerLeveling.AttackMultiplier}x) = Danno Finale: {finalDmg}");
            }
            enemy.TakeDamage(finalDmg, hitDir, _knockbackForce);
            _canDealDamage = false;
        }
    }
    private void TryDamagePlayer(Collider other)
    {
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player == null)
            Debug.Log("WeaponCollision: Non ho trovato PlayerHealth su " + other.name);
        else
            Debug.Log("WeaponCollision: Ho trovato PlayerHealth, infliggo danno!");
        if (player != null)
        {
            float finalDmg = _damage;
            EnemyHealth enemy= transform.root.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                finalDmg = _damage * enemy.DamageMultiplier;
                Debug.Log($"⚔️ Nemico attacca! Danno Base Arma: {_damage} x Moltiplicatore Nemico ({enemy.DamageMultiplier}x) = Danno Finale al Player: {finalDmg}");
            }
            else Debug.Log($"WeaponCollision: Attacco subito dal Player, ma non ho trovato EnemyHealth su {transform.root.name}. Applico danno base.");
            player.TakeDamage(finalDmg);
            _canDealDamage = false;
        }
    }
    #endregion
    #region - weapon Control -
    public void EnableDamage(int enable)
    {
        bool isEnable = (enable == 1);
        _canDealDamage = isEnable;
        if (_weapon != null)
        {
            _weapon.enabled = isEnable;
            Debug.Log(enable == 1 ? " COLLIDER SPADA ACCESO!" : " COLLIDER SPADA SPENTO!");
        }       
        if (_trail != null)
        {
            _trail.emitting = isEnable;
            if (!isEnable) _trail.Clear(); 
        }
    }
    #endregion
}
