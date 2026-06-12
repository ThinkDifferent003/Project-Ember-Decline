using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProstheticPower : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected float _energyCost = 10f;

    protected Camera _mainCamera;
    protected PlayerMovement _playerMovement;
    protected Animator _animator;
    protected PlayerData _playerData;
    protected PlayerEnergy _playerEnergy;

    private ProstheticPower[] _allPowers;

    protected bool _isActive = false;
    #region - Public proprierties -
    public bool IsActive
    {
        get => _isActive;
        protected set
        {
            _isActive = value;
            if (_playerEnergy != null) _playerEnergy.SetRegenerationBlocked(_isActive);
        }
    }
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
            _playerEnergy = _playerMovement.GetComponent<PlayerEnergy>();
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
    protected bool CanActivatePower()
    {
        if (_playerEnergy == null || IsAnyOtherPowerActive())
        {
            Debug.LogWarning($"❌ [{gameObject.name}] Bloccato: C'è un altro potere della protesi già attivo in questo momento.");
            return false;
        }
        bool haveEnergy = _playerEnergy.TryConsumeEnergy(_energyCost);
        if (!haveEnergy)
        {
            Debug.LogWarning($"❌ [{gameObject.name}] Bloccato: Energia insufficiente! Costo: {_energyCost}, Disponibile: {_playerEnergy.CurrentEnergy}");
            return false;
        }
        return true;
    }
    #endregion
    #region - Abstract Methods -
    public abstract void Activate();
    public abstract void ForceDeactivate();
    #endregion
}
