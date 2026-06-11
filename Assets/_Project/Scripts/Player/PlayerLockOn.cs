using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerLockOn : MonoBehaviour
{
    [SerializeField] private float _lockRange = 10f;
    [SerializeField] private GameObject _lockOnIndicator;
    [SerializeField] private CinemachineFreeLook _fCam;
    [SerializeField] private float _lockFOV = 40f;
    [SerializeField] private float _normalFOV = 60f;
    [SerializeField] private float _lockSensitivity = 1f;
    private float _initialXSpeed;
    private float _initialYSpeed;
    private float _initialFOV;
    private GameObject _activeIndicator;
    public Transform CurrentTarget;
    public bool IsLocked => CurrentTarget != null;

    private void Update()
    {
        float targetFOV = (CurrentTarget != null) ? _lockFOV : _normalFOV;
        _fCam.m_Lens.FieldOfView = Mathf.Lerp(_fCam.m_Lens.FieldOfView, targetFOV, Time.deltaTime * 5f);
        if (CurrentTarget == null || (CurrentTarget.TryGetComponent<EnemyHealth>(out var health) && health.IsDead))
        {
            RemoveIndicator();
            CurrentTarget = null;
        }
    }
    public void ToggleLock()
    {
        if (CurrentTarget != null)
        {
            _fCam.m_XAxis.m_MaxSpeed = _initialXSpeed;
            _fCam.m_YAxis.m_MaxSpeed = _initialYSpeed;
            _fCam.m_Lens.FieldOfView = _initialFOV;
            RemoveIndicator();
            CurrentTarget = null;
        }
        else
        {
            FindNearestEnemy();
            if (CurrentTarget != null)
            {
                _initialXSpeed = _fCam.m_XAxis.m_MaxSpeed;
                _initialYSpeed = _fCam.m_YAxis.m_MaxSpeed;
                _initialFOV = _fCam.m_Lens.FieldOfView;

                _fCam.m_XAxis.m_MaxSpeed *= _lockSensitivity;
                _fCam.m_YAxis.m_MaxSpeed *= _lockSensitivity;
                _fCam.m_Lens.FieldOfView += _lockFOV;
                CreateIndicator();
            }
        }
    }
    private void FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position,_lockRange);
        Transform closest = null; 
        float minDist = Mathf.Infinity;
        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("Enemy") && enemy.transform != this.transform)
            {
                float distance = Vector3.Distance(transform.position,enemy.transform.position);
                if (distance < minDist)
                {
                    minDist = distance;
                    closest = enemy.transform;
                }
            }
        }
        CurrentTarget = closest;
    }
    private void CreateIndicator()
    {
        _activeIndicator = Instantiate(_lockOnIndicator, CurrentTarget.position + Vector3.up * 2f, Quaternion.identity);
        _activeIndicator.transform.SetParent(CurrentTarget);
    }
    private void RemoveIndicator()
    {
        if (_activeIndicator != null) Destroy(_activeIndicator);
    }
}
