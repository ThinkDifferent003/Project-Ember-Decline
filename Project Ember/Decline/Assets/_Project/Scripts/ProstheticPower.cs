using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProstheticPower : MonoBehaviour
{
    protected Camera _mainCamera;
    protected Movement _playerMovement;
    protected Animator _animator;
    private ProstheticPower[] _allPowers;
    protected bool _isActive = false;
    public bool IsActive => _isActive;
  
    protected virtual void Start()
    {
        _mainCamera = Camera.main;
        _playerMovement = GetComponentInParent<Movement>();
        _animator = GetComponentInParent<Animator>();
        _allPowers = transform.parent.GetComponentsInChildren<ProstheticPower>();
    }
    protected bool IsAnyOtherPowerActive()
    {
        if (_allPowers ==  null) return false;
        foreach (var power in _allPowers)
        {
            if (power != this && power.IsActive) return true;
        }
        return false;
    }
    protected void SetAnimation(bool active)
    {
        if (_animator != null) _animator.SetBool("IsUsingPower", active);
    }
    public abstract void ForceDeactivate();
}
