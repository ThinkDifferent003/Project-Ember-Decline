using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Configuration")]
    //[SerializeField] private WeaponData _currentWeaponData;
    //[SerializeField] private Transform _handTransform;

    //private GameObject _spawnedWeapon;
    private PlayerAnimation _playerAnim;
    //private WeaponCollision _weaponColl;
    private PlayerStamina _playerStamina;
    private PlayerWeaponHandler _weaponHandler;

    
    private float _staminaCost;
    private int _comboCount;
    
    private bool _comboRegistered;
    private bool _isComboWindowActive;

    private bool _isWeaponEquipped;
    #region - Public Proprierties -
    public bool IsWeaponEquipped => _isWeaponEquipped;
    #endregion

    #region - Lyfe Cycle -
    private void Start()
    {
        _playerAnim = GetComponent<PlayerAnimation>();
        _playerStamina = GetComponent<PlayerStamina>();
        _weaponHandler = GetComponent<PlayerWeaponHandler>();
    }
    private void Update()
    {
        if (Time.timeScale == 0) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) return;
        if (Input.GetKeyDown(KeyCode.G)) ToggleWeapon();
        if (Input.GetMouseButtonDown(0)) TryAttack();
    }
    #endregion
    #region - Core Logic -
    private void TryAttack()
    {
        if (!_playerAnim.CanAttack()) return;
        if (_playerAnim == null) return;
        //if (_currentWeaponData != null) _staminaCost = _currentWeaponData.StaminaCost;
        WeaponData data = _weaponHandler?.GetWeaponData();
        if (data != null) _staminaCost = data.StaminaCost;
        if (!_isWeaponEquipped)
        {
            StartCoroutine(EquipAndAttack());
            return;
        }
        Animator anim = _playerAnim.GetComponent<Animator>();
        if (anim != null)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Idle_Armed") && _playerAnim.IsAttacking)
            {
                Debug.LogWarning("Rilevato disallineamento: l'eroe è in Idle ma lo script era bloccato. Forzo il Reset.");
                ResetComboSequence();
            }
        }
        if (_playerAnim.IsAttacking)
        {
            if (_isComboWindowActive && !_comboRegistered)
            {
                if (_playerStamina != null && _playerStamina.CurrentStamina >= _staminaCost)
                {
                    _comboRegistered = true;
                    Debug.Log($"Click registrato nella finestra! Eseguo subito l'attacco {_comboCount + 1}");
                    ExecuteAttack();
                    return;
                }
            }
            return;
        }
        _comboCount = 0;
        ExecuteAttack();
    }
    private void ExecuteAttack()
    {
        if (_playerStamina != null && !_playerStamina.TryConsumeStamina(_staminaCost)) return;
        _comboCount++;
        if (_comboCount > 2) _comboCount = 1;
        _comboRegistered = false;
        _isComboWindowActive = false;
        if (_playerAnim != null) _playerAnim.TriggerAttack(_comboCount);
        
        
    }
    private void ToggleWeapon()
    {
        //if (_currentWeaponData == null) return;
        if (_weaponHandler == null || _weaponHandler.GetWeaponData() == null) return;
        _isWeaponEquipped = !_isWeaponEquipped;
        if (_isWeaponEquipped) _weaponHandler.EquipWeapon(_playerAnim); //EquipWeapon();
        else _weaponHandler.UnequipWeapon();//UnequipWeapon();
        _playerAnim?.SetWeaponEquippedState(_isWeaponEquipped, _weaponHandler.GetSpwnedWeapon());//_spawnedWeapon );
    }
    #endregion
    #region - Animation Events -
    public void StartAttackCollision()
    {
        //_weaponColl?.EnableDamage(1);
        _weaponHandler?.UpdateCollisionDamage();
        _weaponHandler?.GetWeaponCollision()?.EnableDamage(1);
    }
    public void EndAttackCollision()
    {
        //_weaponColl?.EnableDamage(0);
        _weaponHandler?.GetWeaponCollision()?.EnableDamage(0);
    }
    public void OnComboWindowOpen()
    {
        Debug.Log("--- FINESTRA APERTA: PREMI ORA! ---");
        _isComboWindowActive = true;
        if (_playerAnim != null)
        {
            Animator anim = _playerAnim.GetComponent<Animator>();
            if (anim != null) anim.SetFloat("AttackSpeed", 0.4f);
        }
        CancelInvoke(nameof(OnComboWindowClose));
        Invoke(nameof(OnComboWindowClose), 5f);
    }
    public void OnComboWindowClose()
    {
        if(_isComboWindowActive && !_comboRegistered) _isComboWindowActive = false;
        if (_playerAnim != null)
        {
            Animator anim = _playerAnim.GetComponent<Animator>();
            if (anim != null) anim.SetFloat("AttackSpeed", 1.0f);
        }
    }    
    public void ResetComboSequence()
    {
        CancelInvoke(nameof(OnComboWindowClose));
        _comboCount = 0;
        _comboRegistered = false;
        _isComboWindowActive= false;
        if (_playerAnim != null)
        {
            Animator anim = _playerAnim.GetComponent<Animator>();
            if (anim != null) anim.SetFloat("AttackSpeed", 1.0f);
            _playerAnim.ResetAttack();
        }
        Debug.Log("--- COMBO RESETTATA COMPLETAMENTE ---");
    }
    #endregion
    #region - Coroutines -
    private IEnumerator EquipAndAttack()
    {
        ToggleWeapon();
        yield return null;
        TryAttack();
    }
    #endregion
}
