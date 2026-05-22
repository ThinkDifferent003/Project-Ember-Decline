using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ProstheticTelekinesis : ProstheticPower
{
    [SerializeField] private float _maxGrabDistance = 15f;
    [SerializeField] private Transform _armSource;
    [SerializeField] private LayerMask _grabbyLayer;
    [SerializeField] private float _attractSpeed = 10f;
    [SerializeField] private float _throwForce = 30f;
    [SerializeField] private float _pullForce = 0.05f;
    [SerializeField] private float _liftHeight = 2f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private LineRenderer _lineRenderer;
    private Rigidbody _grabbedObj;
    private Vector3 _relativeOffset;
    private Animator _anim;
    private bool _isStabilized = false;

    protected override void Start()
    {
        base.Start();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_isActive) TryGrab();
            else Release();
        }
        if (_isActive && _grabbedObj != null)
        {
            UpdateLineRenderer();
            if (Input.GetMouseButtonDown(0)) Throw();
            else if (Input.GetMouseButton(1)) PullCloser();
        }
        else
        {
            if (_lineRenderer.enabled) _lineRenderer.enabled = false;
        }
    }
    private void FixedUpdate()
    {
        if (_isActive && _grabbedObj != null)
        {
            Vector3 targetPos = _playerMovement.transform.TransformPoint(_relativeOffset);
            Vector3 diff = targetPos - _grabbedObj.transform.position;
            if (!_isStabilized && diff.magnitude < 0.5f) _isStabilized = true; 
            if (_isStabilized) RotatePlayerToMouse();
            _grabbedObj.velocity = diff * _attractSpeed;
            _grabbedObj.angularVelocity = Vector3.Lerp(_grabbedObj.angularVelocity,Vector3.zero,Time.fixedDeltaTime * 5f);
        }
    }
    private void TryGrab()
    {
        if (IsAnyOtherPowerActive()) return;   
        Vector3 origin = _armSource.position;
        Vector3 dir = _playerMovement.transform.forward;
        RaycastHit hit;
        if (Physics.SphereCast(origin, 0.5f, dir , out hit , _maxGrabDistance,_grabbyLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                _grabbedObj = rb;
                _grabbedObj.useGravity = false;
                Vector3 liftPos = _grabbedObj.position + Vector3.up * _liftHeight;
                _relativeOffset = _playerMovement.transform.InverseTransformPoint(liftPos);
                _isActive = true;
                SetAnimation(true);
            }
        }
    }
    private void Release()
    {
        if (_grabbedObj != null)
        {
            _grabbedObj.useGravity = true;
            _grabbedObj = null;
        }
        _isActive = false;
        _lineRenderer.enabled = false;
        SetAnimation(false);
    }
    private void Throw()
    {
        Rigidbody rbTothrow = _grabbedObj;
        _isActive = false;
        SetAnimation(false);
        rbTothrow.useGravity = true;
        Vector3 throwDir = _playerMovement.transform.forward;
        rbTothrow.velocity = Vector3.zero;
        rbTothrow.AddForce(throwDir * _throwForce, ForceMode.Impulse);
        _grabbedObj = null;
    }
    private void PullCloser()
    {
        if (_grabbedObj == null) return;
        Vector3 targetAnchor = new Vector3(0f, _relativeOffset.y, 1f);
        float currentDist = Vector3.Distance(_relativeOffset, targetAnchor);
        if (currentDist > 0.1f)
        {
            _relativeOffset = Vector3.MoveTowards(_relativeOffset,targetAnchor,_pullForce * Time.deltaTime);
        }
    }
    private void RotatePlayerToMouse()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(Vector3.up,_playerMovement.transform.position);
        float distance;
        if (playerPlane.Raycast(ray, out distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            Vector3 lookDir = (targetPoint - _playerMovement.transform.position).normalized;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                _playerMovement.transform.rotation = Quaternion.Slerp(_playerMovement.transform.rotation, targetRot, Time.fixedDeltaTime * _rotationSpeed);
            }
        }
    }
    private void UpdateLineRenderer()
    {
        if (!_lineRenderer.enabled) _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, _armSource.position);
        _lineRenderer.SetPosition(1,_grabbedObj.worldCenterOfMass);
    }
    public override void ForceDeactivate()
    {
        Release();
    }
}
