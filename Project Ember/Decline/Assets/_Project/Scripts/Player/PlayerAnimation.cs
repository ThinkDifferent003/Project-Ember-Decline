using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private WeaponData _currentWeaponData;
    [SerializeField] private Transform _handTransform;
    
    
    private Animator _animator;
    private Movement _movement;
    private GameObject _spawnedWeapon;
    private bool _isWeaponEquipped;
    private Transform _activeWeaponHandleTarget;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<Movement>();
    }
    private void Update()
    {
        if (_movement == null || _animator == null) return;
        float currentSpeed = _movement.IsMoving ? 1f : 0f;
        _animator.SetFloat("Speed", currentSpeed);
    }
    private void LateUpdate()
    {
        if (_isWeaponEquipped && _spawnedWeapon != null)
        {
            Transform handContainer = _spawnedWeapon.transform.Find("Posa_Mano");
            if (handContainer != null) ApplicationFingerPose(handContainer, _handTransform);
        }
    } 
    public void ApplyWeaponOverride(AnimatorOverrideController overrideController)
    {
        if (_animator != null) _animator.runtimeAnimatorController = overrideController;
    }
    public void SetWeaponEquippedState(bool isEquipped , GameObject weaponObj)
    {
        _isWeaponEquipped = isEquipped;
        _spawnedWeapon = weaponObj;
        if (_animator != null) _animator.SetBool("IsWeaponEquipped", _isWeaponEquipped);
    }
    public void TriggerAttack()
    {
        if (_animator != null) _animator.SetTrigger("Attack");
    }
    public void TriggerHurt()
    {
        if (_animator != null) _animator.SetTrigger("Hurt");
    }
    public bool CanAttack()
    {
        if (_animator == null) return false;
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Idle_Unarmed") || stateInfo.IsName("Walking") || stateInfo.IsName("Idle_Armed"))
        {
            return true;
        }
        else if (stateInfo.IsName("Attack_1")) return stateInfo.normalizedTime > 0.1f;
        return false;
    }
    private void ApplicationFingerPose(Transform sorgent, Transform destination)
    {
        foreach (Transform finger in sorgent)
        {
            Transform fingerDestination = destination.Find(finger.name);
            if (fingerDestination != null)
            {
                fingerDestination.localRotation = finger.localRotation;
                ApplicationFingerPose(finger, fingerDestination);
            }
        }
    }
    
    
    
}
