using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProstheticPower : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected float _energyCost = 15f;

    protected Camera _mainCamera;
    protected PlayerMovement _playerMovement;
    protected Animator _animator;
    protected PlayerData _playerData;

    private ProstheticPower[] _allPowers;

    protected bool _isActive = false;
    #region - Public proprierties -
    public bool IsActive => _isActive;
    #endregion

    #region - Lyfe Cycle -
    protected virtual void Start()
    {
        _mainCamera = Camera.main;
        _playerMovement = GetComponentInParent<PlayerMovement>();
        _animator = GetComponentInParent<Animator>();
        if (transform.parent != null) _allPowers = transform.parent.GetComponentsInChildren<ProstheticPower>();
        if (_playerMovement != null)
        {
            PlayerSetup setup = _playerMovement.GetComponent<PlayerSetup>();
            if (setup != null) _playerData = setup.PlayerData;
        }
    }
    #endregion
    #region - Core Logic -
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
        _animator?.SetBool("IsUsingPower", active);
    }
    #endregion
    #region - Abstract Methods -
    public abstract void Activate();
    public abstract void ForceDeactivate();
    #endregion
}
