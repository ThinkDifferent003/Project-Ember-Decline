using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private Transform _handTransform;
    [HideInInspector] public WeaponData _currentWeapon;
    private GameObject _spawnedWeapon;
    private WeaponCollision _weaponColl;
    private PlayerLeveling _playerLeveling;

    private void Awake()
    {
        _playerLeveling = GetComponent<PlayerLeveling>();
        if (_weaponData != null) _currentWeapon = ScriptableObject.Instantiate(_weaponData);
    }
    public WeaponData GetWeaponData() => _currentWeapon;
    public GameObject GetSpwnedWeapon() => _spawnedWeapon;
    public WeaponCollision GetWeaponCollision() => _weaponColl;
    public void EquipWeapon(PlayerAnimation playerAnim)
    {
        if (_currentWeapon == null) return;
        if (playerAnim != null && _currentWeapon.WeaponOverride != null) playerAnim.ApplyWeaponOverride(_currentWeapon.WeaponOverride);
        if (_spawnedWeapon == null && _currentWeapon.WeaponPrefab != null)
        {
            _spawnedWeapon = Instantiate(_currentWeapon.WeaponPrefab, _handTransform);
            _spawnedWeapon.transform.localPosition = Vector3.zero;
            _spawnedWeapon.transform.localRotation = Quaternion.identity;
            _weaponColl = _spawnedWeapon.GetComponentInChildren<WeaponCollision>();
            UpdateCollisionDamage();
        }
        else if (_spawnedWeapon != null)
        {
            _spawnedWeapon.SetActive(true);
            UpdateCollisionDamage();
        }
    }
    public void UnequipWeapon()
    {
        if (_spawnedWeapon != null) _spawnedWeapon.SetActive(false);
    }
    public void UpdateCollisionDamage()
    {
        if (_weaponColl != null && _currentWeapon != null)
        {
            float multiplier = _playerLeveling != null ? _playerLeveling.AttackMultiplier : 1.0f;
            float finalDmg = _currentWeapon.Damage * multiplier;
            _weaponColl.InizializeWeaponData(finalDmg, _currentWeapon.KnockbackForce);
        }
    }
    public void EnhancementWeapon(float increaseDmg)
    {
        if (_currentWeapon != null)
        {
            
        }
    }
}
