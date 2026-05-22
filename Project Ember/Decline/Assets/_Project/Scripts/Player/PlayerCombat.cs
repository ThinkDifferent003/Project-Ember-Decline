using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private WeaponData _currentWeaponData;
    [SerializeField] private Transform _handTransform;
    private GameObject _spawnedWeapon;
    private bool _isWeaponEquipped;
    private PlayerAnimation _playerAnim;
    public bool IsWeaponEquipped => _isWeaponEquipped;

    private void Start()
    {
        _playerAnim = GetComponent<PlayerAnimation>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) ToggleWeapon();
        if (Input.GetMouseButtonDown(0)) AttackInput();
    }
    private void AttackInput()
    {
        if (_playerAnim == null) return;
        if (!_playerAnim.CanAttack()) return;
        if (!_isWeaponEquipped)
        {
            StartCoroutine(EquipAndAttack());
            return;
        }
        if (_playerAnim != null) _playerAnim.TriggerAttack();
    }
    private void ToggleWeapon()
    {
        if (_currentWeaponData == null) return;

        _isWeaponEquipped = !_isWeaponEquipped;

        if (_isWeaponEquipped)
        {
            if (_playerAnim != null && _currentWeaponData.WeaponOverride != null)
                _playerAnim.ApplyWeaponOverride(_currentWeaponData.WeaponOverride);

            if (_spawnedWeapon == null && _currentWeaponData.WeaponPrefab != null)
            {
                _spawnedWeapon = Instantiate(_currentWeaponData.WeaponPrefab, _handTransform);
                _spawnedWeapon.transform.localPosition = Vector3.zero;
                _spawnedWeapon.transform.localRotation = Quaternion.identity;
                WeaponCollision weaponCol = _spawnedWeapon.GetComponentInChildren<WeaponCollision>();
                if (weaponCol != null) weaponCol.InizializeWeaponData(_currentWeaponData.Damage, _currentWeaponData.KnockbackForce);
            }
            else if (_spawnedWeapon != null)
            {
                _spawnedWeapon.SetActive(true);
            }
        }
        else
        {
            if (_spawnedWeapon != null) _spawnedWeapon.SetActive(false);
        }
        if (_playerAnim != null) _playerAnim.SetWeaponEquippedState(_isWeaponEquipped, _spawnedWeapon);
    }
    public void StartAttackCollision()
    {
        if (_spawnedWeapon != null)
        {
            WeaponCollision weaponCol = _spawnedWeapon.GetComponentInChildren<WeaponCollision>(true);
            if (weaponCol != null) weaponCol.EnableDamage(1);
        }
    }
    public void EndAttackCollision()
    {
        if (_spawnedWeapon != null)
        {
            WeaponCollision weaponCol = _spawnedWeapon.GetComponentInChildren<WeaponCollision>(true);
            if (weaponCol != null) weaponCol.EnableDamage(0);
        }
    }
    private IEnumerator EquipAndAttack()
    {
        ToggleWeapon();
        yield return null;
        if (_playerAnim != null) _playerAnim.TriggerAttack();
    }
}
