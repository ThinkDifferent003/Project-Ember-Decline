using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProstheticFlame : ProstheticPower
{
    [Header("Settings")]
    [SerializeField] private float _damagePerSecond = 0f;
    [Header("Components")]
    [SerializeField] private ParticleSystem _flameParticles;
    private Collider _coll;

    #region - Lyfe Cycle -
    protected override void Start()
    {
        base.Start();
        _coll = GetComponent<Collider>();
        ToggleFlame(false);
    }
    #endregion
    #region - Core Logic -
    public void ToggleFlame(bool activated)
    {
        if (activated && _isActive) return;
        if (activated && IsAnyOtherPowerActive()) return; 
        _isActive = activated;
        if (_coll != null) _coll.enabled = _isActive;
        if (_flameParticles != null)
        {
            if (_isActive) _flameParticles.Play();
            else _flameParticles.Stop();
        }
        Debug.Log(_isActive ? "Attivo" : "Disattivo");
        SetAnimation(_isActive);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!_isActive) return;
        IBurnable burnable = other.GetComponent<IBurnable>();
        if (burnable != null) burnable.Burn(_damagePerSecond *  Time.deltaTime);
    }
    #endregion
    #region - Utility -
    public override void Activate()
    {
        ToggleFlame(true);
    }
    public override void ForceDeactivate()
    {
        ToggleFlame(false);
    }
    #endregion
}
