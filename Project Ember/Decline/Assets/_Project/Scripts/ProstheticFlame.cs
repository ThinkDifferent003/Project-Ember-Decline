using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProstheticFlame : ProstheticPower
{
    [SerializeField] private float _damagePerSecond = 20f;
    [SerializeField] private ParticleSystem _flameParticles;
    private Collider _coll;

    protected override void Start()
    {
        base.Start();
        _coll = GetComponent<Collider>();
        ToggleFlame(false);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Q)) 
        {
            if (!_isActive) ToggleFlame(true); 
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (_isActive) ToggleFlame(false);
        }
    }
    public void ToggleFlame(bool activated)
    {
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
    public override void ForceDeactivate()
    {
        ToggleFlame(false);
    }
}
