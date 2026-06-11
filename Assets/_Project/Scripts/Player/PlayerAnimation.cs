using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private PlayerMovement _movement;
    private GameObject _spawnedWeapon;

    private bool _isWeaponEquipped;
    public bool IsAttacking { get; private set; }

    #region - Lyce Cycle - 
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();
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
            if (handContainer != null)
            {
                Transform handDestination = _animator.GetBoneTransform(HumanBodyBones.RightHand);
                if (handDestination != null) ApplicationFingerPose(handContainer, handDestination);
            }
        }
    }
    #endregion
    #region - Animation & Combat -
    public void ApplyWeaponOverride(AnimatorOverrideController overrideController)
    {
        if (_animator != null) _animator.runtimeAnimatorController = overrideController;
    }
    public void SetWeaponEquippedState(bool isEquipped , GameObject weaponObj)
    {
        _isWeaponEquipped = isEquipped;
        _spawnedWeapon = weaponObj;
        _animator?.SetBool("IsWeaponEquipped", _isWeaponEquipped);
    }
    public void TriggerAttack(int comboIndex)
    {
        IsAttacking = true;
        _animator.SetInteger("ComboIndex", comboIndex);
        _animator?.SetTrigger("Attack");
    }
    public void TriggerHurt()
    {
        _animator?.SetTrigger("Hurt");
    }
    public bool CanAttack()
    {
        if (_animator == null) return false;
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Idle_Unarmed") || stateInfo.IsName("Walking") || stateInfo.IsName("Idle_Armed")) return true;
        if (stateInfo.IsTag("Attack")) return stateInfo.normalizedTime > 0.2f;
        return false;
    }
    public void ResetAttack()
    {
        IsAttacking = false;
        _animator.SetInteger("ComboIndex", 0);
    }
    #endregion
    #region - Utility -
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
    #endregion
}
