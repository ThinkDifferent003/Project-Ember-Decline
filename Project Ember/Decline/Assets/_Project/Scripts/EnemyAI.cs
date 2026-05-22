using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject _weaponObj;
    [SerializeField] private WeaponData _weaponData;
    private EnemyData _enemyData;
    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private float _colldownTimer;
    private bool _hasEquippedSword = false;
    private EnemyAnimation _enemyAnim;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _enemyAnim = GetComponent<EnemyAnimation>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;
        if (_weaponObj != null)
        {
            _weaponObj.SetActive(false);
            WeaponCollision weaponCol = _weaponObj.GetComponentInChildren<WeaponCollision>(true);
            if (weaponCol != null && _enemyData != null) weaponCol.InizializeWeaponData(_weaponData.Damage, _weaponData.KnockbackForce);
        }
    }
    public void Inizialize(EnemyData data)
    {
        _enemyData = data;
    }
    private void Update()
    {
        if (_enemyData == null || _playerTransform == null || _agent == null) return;
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
        if (_colldownTimer > 0 ) _colldownTimer -= Time.deltaTime;
        if (distanceToPlayer <= _enemyData.VisualRange)
        {
            if (!_hasEquippedSword) EquipWeapon();
            if (distanceToPlayer <= _enemyData.AttackRange)
            {
                _agent.isStopped = true;
                if (_colldownTimer <= 0) Attack();
            }
            else
            {
                _agent.isStopped = false;
                _agent.SetDestination(_playerTransform.position);
            }
        }
        else
        {
            if (_hasEquippedSword) UnequipWeapon();
            if (_agent.hasPath) _agent.ResetPath();
        }
    }
    private void EquipWeapon()
    {
        _hasEquippedSword = true;
        if (_weaponObj != null) _weaponObj.SetActive(true);
        if (_enemyAnim != null) _enemyAnim.SetEquippedState(true);
    }
    private void UnequipWeapon()
    {
        _hasEquippedSword = false;
        if (_weaponObj != null) _weaponObj.SetActive(false);
        if (_enemyAnim != null) _enemyAnim.SetEquippedState(false);
    }
    private void Attack()
    {
        _colldownTimer = _enemyData.AttackCooldown;
        if (_agent.hasPath) _agent.ResetPath();
        Vector3 dir = (_playerTransform.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir); 
        if (_enemyAnim != null) _enemyAnim.TriggerAttackAnimation();
    }
    public void StartAttackCollision()
    {
        if (_weaponObj != null)
        {
            WeaponCollision weaponCol = _weaponObj.GetComponentInChildren<WeaponCollision>(true);
            if (weaponCol != null) weaponCol.EnableDamage(1);
        }
    }
    public void EndAttackCollision()
    {
        if (_weaponObj != null)
        {
            WeaponCollision weaponCol = _weaponObj.GetComponentInChildren<WeaponCollision>(true);
            if (weaponCol != null) weaponCol.EnableDamage(0);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (this == null) return;
        if (_enemyData != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _enemyData.VisualRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _enemyData.AttackRange);
            return;
        }
        EnemySetup setup = GetComponent<EnemySetup>();
        if (setup != null && setup.Stats != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, setup.Stats.VisualRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, setup.Stats.AttackRange);
        }        
    }
}
