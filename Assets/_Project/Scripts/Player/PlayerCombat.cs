using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private WeaponData _currentWeaponData;
    [SerializeField] private Transform _handTransform;

    private GameObject _spawnedWeapon;
    private PlayerAnimation _playerAnim;
    private WeaponCollision _weaponColl;
    private PlayerStamina _playerStamina;

    private bool _bufferNextAttack;
    private float _staminaCost;
    private int _comboCount;
    private Coroutine _comboCoroutine;

    private bool _isWeaponEquipped;
    #region - Public Proprierties -
    public bool IsWeaponEquipped => _isWeaponEquipped;
    #endregion

    #region - Lyfe Cycle -
    private void Start()
    {
        _playerAnim = GetComponent<PlayerAnimation>();
        _playerStamina = GetComponent<PlayerStamina>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) ToggleWeapon();
        if (Input.GetMouseButtonDown(0)) TryAttack();
    }
    #endregion
    #region - Core Logic -
    private void TryAttack()
    {
        if (!_playerAnim.CanAttack()) return;
        if (_currentWeaponData != null) _staminaCost = _currentWeaponData.StaminaCost;
        if (!_isWeaponEquipped)
        {
            StartCoroutine(EquipAndAttack());
            return;
        }
        if (_playerAnim.IsAttacking && _comboCount == 0) _playerAnim.ResetAttack();
        if (!_playerAnim.IsAttacking)
        {
            if (_playerStamina != null && !_playerStamina.TryConsumeStamina(_staminaCost)) return;
            _comboCount = 1;
            _playerAnim.TriggerAttack(_comboCount);
            if (_comboCoroutine != null) StopCoroutine(_comboCoroutine);
            _comboCoroutine = StartCoroutine(ComboRoutine());
        }
        else
        {
            if (_playerStamina != null && _playerStamina.CurrentStamina >= _staminaCost) _bufferNextAttack = true;
        }
    }
    private void ToggleWeapon()
    {
        if (_currentWeaponData == null) return;
        _isWeaponEquipped = !_isWeaponEquipped;
        if (_isWeaponEquipped) EquipWeapon();
        else UnequipWeapon();
        _playerAnim?.SetWeaponEquippedState(_isWeaponEquipped, _spawnedWeapon);
    }
    #endregion
    #region - Weapon Management -
    private void EquipWeapon()
    {
        if (_playerAnim != null && _currentWeaponData.WeaponOverride != null)
            _playerAnim.ApplyWeaponOverride(_currentWeaponData.WeaponOverride);

        if (_spawnedWeapon == null && _currentWeaponData.WeaponPrefab != null)
        {
            _spawnedWeapon = Instantiate(_currentWeaponData.WeaponPrefab, _handTransform);
            _spawnedWeapon.transform.localPosition = Vector3.zero;
            _spawnedWeapon.transform.localRotation = Quaternion.identity;
            _weaponColl = _spawnedWeapon.GetComponentInChildren<WeaponCollision>();
            if (_weaponColl != null) _weaponColl.InizializeWeaponData(_currentWeaponData.Damage, _currentWeaponData.KnockbackForce);
        }
        else if (_spawnedWeapon != null) _spawnedWeapon.SetActive(true);
    }
    private void UnequipWeapon()
    {
        if (_spawnedWeapon != null) _spawnedWeapon.SetActive(false);
    }
    #endregion
    #region - Animation Events -
    public void StartAttackCollision()
    {
        _weaponColl?.EnableDamage(1);
    }
    public void EndAttackCollision()
    {
        _weaponColl?.EnableDamage(0);
    }
    #endregion
    #region - Coroutines -
    private IEnumerator EquipAndAttack()
    {
        ToggleWeapon();
        yield return null;
        TryAttack();
    }
    private IEnumerator ComboRoutine()
    {
        float windowStart = 0.3f;
        float windowEnd = 0.9f;
        float elapsed = 0f;
        _bufferNextAttack = false;
        while (elapsed  < windowEnd)
        {
            elapsed += Time.deltaTime;
            if (elapsed >=  windowStart && _bufferNextAttack)
            {
                if (_playerStamina != null && !_playerStamina.TryConsumeStamina(_staminaCost))
                {
                    _bufferNextAttack = false;
                    yield break;
                }
                _bufferNextAttack = false;
                _comboCount++;
                _playerAnim.TriggerAttack(_comboCount);
                yield break;
            }
            yield return null;
        }
    }
    #endregion
}
