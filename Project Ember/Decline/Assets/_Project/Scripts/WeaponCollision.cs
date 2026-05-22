using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    private float _damage;
    private float _knockbackForce;
    private Collider _weapon;
    private bool _canDealDamage = false;
    private TrailRenderer _trail;

    private void Start()
    {
        _weapon = GetComponent<Collider>();
        _trail = GetComponentInChildren<TrailRenderer>();
        EnableDamage(0);
    }
    public void InizializeWeaponData(float damage , float knockback)
    {
        _damage = damage;
        _knockbackForce = knockback;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_canDealDamage) return;
        if (other.gameObject == transform.root.gameObject) return;
        Vector3 hitDir = other.transform.position - transform.position;
        hitDir.y = 0;
        if (hitDir == Vector3.zero) hitDir = transform.forward;
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null) enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damage,hitDir,_knockbackForce);
            _canDealDamage = false;
            return;
        }
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player == null) player = other.GetComponentInParent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(_damage);
            _canDealDamage = false;
        }
    }
    public void EnableDamage(int enable)
    {
        _canDealDamage = (enable == 1);
        if (_weapon != null)
        {
            _weapon.enabled = (enable == 1);
            Debug.Log(enable == 1 ? " COLLIDER SPADA ACCESO!" : " COLLIDER SPADA SPENTO!");
        }       
        if (_trail != null)
        {
            _trail.emitting = (enable == 1);
            if (enable == 0) _trail.Clear(); 
        }
    }
}
