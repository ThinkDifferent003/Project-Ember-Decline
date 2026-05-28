using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public abstract class EnemyAI : MonoBehaviour
{
    [Header("Weapon Config")]
    [SerializeField] protected GameObject _weaponObj;
    [SerializeField] protected WeaponData _weaponData;   

    protected NavMeshAgent _agent;
    protected Transform _playerTransform;
    protected EnemyAnimation _enemyAnim;
    protected EnemyHealth _enemyHealth;
    protected PlayerHealth _playerHealth;

    protected EnemyData _enemyData;
    protected float _colldownTimer;
    protected bool _hasEquippedSword = false;

    private WeaponCollision _weaponColl;

    #region - Lyfe Cycle -
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _enemyAnim = GetComponent<EnemyAnimation>();
        _enemyHealth = GetComponent<EnemyHealth>();
        if (_enemyHealth != null) _agent.updateRotation = false;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
            _playerHealth = player.GetComponent<PlayerHealth>();
        }
        if (_weaponObj != null)
        {
            _weaponObj.SetActive(false);
            _weaponColl = _weaponObj.GetComponentInChildren<WeaponCollision>(true);
        }
    }
    protected void Update()
    {
        if (_enemyData == null || _playerTransform == null) return;
        if (_enemyHealth != null && _enemyHealth.IsStunned) return;
        _colldownTimer -= Time.deltaTime;
        PerformLogic();
    }
    #endregion
    #region - Cambat Logic -
    protected abstract void PerformLogic(); 
    protected void RotateTowardsPlayer()
    {
        Vector3 dir = (_playerTransform.position - transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }
    protected void Attack()
    {
        _colldownTimer = _enemyData.AttackCooldown;
        transform.LookAt(new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z));
        _enemyAnim?.TriggerAttackAnimation();
    }
    #endregion
    #region - Weapon Logic - 
    protected void SetEquippedState(bool isEquipped)
    {
        _hasEquippedSword = isEquipped;
        _weaponObj?.SetActive(isEquipped);
        _enemyAnim?.SetEquippedState(isEquipped);
    } 
    public void StartAttackCollision()
    {
        _weaponColl?.EnableDamage(1);
    }
    public void EndAttackCollision()
    {
        _weaponColl?.EnableDamage(0);
    }
    #endregion
    #region - Utility -
    protected float GetPlayerHealth()
    {
        if (_playerHealth == null) return 1.0f;
        return _playerHealth.CurrentHealth / _playerHealth.MaxHealth;
    }
    public void Inizialize(EnemyData data)
    {
        _enemyData = data;
        if (_weaponObj != null && _weaponData != null)
        {
            WeaponCollision weaponCol = _weaponObj.GetComponentInChildren<WeaponCollision>(true);
            if (weaponCol != null && _enemyData != null) weaponCol.InizializeWeaponData(_weaponData.Damage, _weaponData.KnockbackForce);
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
    #endregion
}
