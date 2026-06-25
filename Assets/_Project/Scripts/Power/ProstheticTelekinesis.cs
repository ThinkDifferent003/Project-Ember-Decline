using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class ProstheticTelekinesis : ProstheticPower
{
    [Header("Physics & Movement")]
    [SerializeField] private float _maxGrabDistance = 15f;
    [SerializeField] private float _attractSpeed = 10f;
    [SerializeField] private float _throwForce = 30f;
    [SerializeField] private float _pullForce = 0.05f;
    [SerializeField] private float _liftHeight = 2f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private float _objectWeight = 1.0f;
    [Header("Components")]
    [SerializeField] private Transform _armSource;
    [SerializeField] private LayerMask _grabbyLayer;
    [SerializeField] private LineRenderer _lineRenderer;
    private Rigidbody _grabbedObj;
    private Collider _jailColl;
    private Vector3 _relativeOffset;
    private bool _isStabilized = false;

    #region - Lyfe Cycle -
    protected override void Start()
    {
        base.Start();
        if (_lineRenderer != null) _lineRenderer.enabled = false;
    }
    private void Update()
    {
        if (_isActive && _grabbedObj != null)
        {
            if (_playerEnergy != null && _playerEnergy.CurrentEnergy <= 0f)
            {
                Release();
                return;
            }
            UpdateLineRenderer();
            HandleInput();
        }
        else if (_lineRenderer != null && _lineRenderer.enabled) _lineRenderer.enabled = false;
    }
    private void FixedUpdate()
    {
        if (_isActive && _grabbedObj != null) ApplyTelekinesisPhysics();
    }
    #endregion
    #region - Core Logic -
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) Throw();
        else if (Input.GetMouseButton(1)) PullCloser();
    }
    private void ApplyTelekinesisPhysics()
    {
        Vector3 targetPos = _playerMovement.transform.TransformPoint(_relativeOffset);
        Vector3 diff = targetPos - _grabbedObj.transform.position;
        if (!_isStabilized && diff.magnitude < 0.5f) _isStabilized = true;
        if (_isStabilized) RotatePlayerToMouse();
        if (_grabbedObj.isKinematic)
        {
            if (_grabbedObj.TryGetComponent<EnemyLiftable>(out EnemyLiftable enemy) && _grabbedObj.isKinematic)
            {
                _objectWeight = enemy.Weight;
            }
            _grabbedObj.MovePosition(Vector3.Lerp(_grabbedObj.position, targetPos, Time.fixedDeltaTime * _attractSpeed / _objectWeight));
        } 
        else
        {
            _grabbedObj.velocity = (diff * _attractSpeed) / _objectWeight;
            _grabbedObj.angularVelocity = Vector3.Lerp(_grabbedObj.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 5f);
        }
    }
    #endregion
    #region - Actions -
    private void TryGrab()
    {
        if (!CanActivatePower()) return;
        Vector3 origin = _armSource.position;
        Vector3 dir = _playerMovement.transform.forward;
        RaycastHit hit;
        if (Physics.SphereCast(origin, 0.5f, dir , out hit , _maxGrabDistance,_grabbyLayer))
        {
            EnemyLiftable enemyLift = hit.collider.GetComponent<EnemyLiftable>();
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            
            if (enemyLift != null)
            {
                _objectWeight = enemyLift.Weight;
                var ai = enemyLift.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.StopAllCoroutines();
                    ai.enabled = false;
                }
                if (enemyLift.TryGetComponent<UnityEngine.AI.NavMeshAgent>(out var agent)) agent.enabled = false;
                if (enemyLift.TryGetComponent<EnemyDefensive>(out EnemyDefensive enemyDefensive))
                {
                    enemyDefensive.StopAllCoroutines();
                    enemyDefensive.SetTelekinesisDisable(true);
                }
               
                _grabbedObj = rb;
                _grabbedObj.isKinematic = true;
                _grabbedObj.useGravity = false;
                Vector3 liftPos = _grabbedObj.position; //+ Vector3.up * _liftHeight;
                //if (hit.collider.GetComponent<ItemHook>() == null) liftPos += Vector3.up * _liftHeight;
                _relativeOffset = _playerMovement.transform.InverseTransformPoint(liftPos);
                IsActive = true;
                SetAnimation(true);
            }
            else if (rb != null)
            {
                _grabbedObj = rb;
                _grabbedObj.isKinematic = false;
                _grabbedObj.useGravity = false;
                Vector3 liftPos = _grabbedObj.position + Vector3.up * _liftHeight;
                _relativeOffset = _playerMovement.transform.InverseTransformPoint(liftPos);
                IsActive = true;
                SetAnimation(true);
            }
        }
    }
    private void Release()
    {
        if (_grabbedObj != null)
        {
            var ai = _grabbedObj.GetComponent<EnemyAI>();
            if (ai != null) ai.enabled = true;
            if (_grabbedObj.TryGetComponent<UnityEngine.AI.NavMeshAgent>(out var agent)) agent.enabled = true;
            if (_grabbedObj.TryGetComponent<EnemyDefensive>(out EnemyDefensive enemyDefensive)) enemyDefensive.SetTelekinesisDisable(false); 
            _grabbedObj.isKinematic = false;
            _grabbedObj.useGravity = true;
            _grabbedObj.velocity = Vector3.zero;
            _grabbedObj = null;
        }
        IsActive = false;
        _isStabilized = false;
        if (_lineRenderer != null) _lineRenderer.enabled = false;
        SetAnimation(false);
    }
    private void Throw()
    {
        if (_grabbedObj == null) return;
        _grabbedObj.isKinematic = false;
        
        _grabbedObj.useGravity = true;
        _grabbedObj.AddForce(_playerMovement.transform.forward * _throwForce, ForceMode.Impulse);
        var ai = _grabbedObj.GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = true;
        if (_grabbedObj.TryGetComponent<UnityEngine.AI.NavMeshAgent>(out var agent)) agent.enabled = true;
        if (_grabbedObj.TryGetComponent<EnemyDefensive>(out EnemyDefensive enemyDefensive)) enemyDefensive.SetTelekinesisDisable(false);
        if (_grabbedObj.TryGetComponent<TelekinesisObstacle>(out TelekinesisObstacle obstacle)) obstacle.Launch();
        _grabbedObj = null;
        IsActive = false;
        _isStabilized = false;
        if (_lineRenderer != null) _lineRenderer.enabled = false;
        SetAnimation(false);    
    }
    private void PullCloser()
    {
        Vector3 targetAnchor = new Vector3(0f, _relativeOffset.y, 1f);
        _relativeOffset = Vector3.MoveTowards(_relativeOffset,targetAnchor,_pullForce * Time.deltaTime);
    }
    #endregion
    #region - Utility -
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
        if (_lineRenderer == null) return;
        if (!_lineRenderer.enabled) _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, _armSource.position);
        Vector3 targetPoint = _grabbedObj.transform.position;
        if (_grabbedObj.TryGetComponent<Collider>(out Collider col)) targetPoint = col.bounds.center;
        //if (_grabbedObj.GetComponent<ItemHook>() != null) _lineRenderer.SetPosition(1, _grabbedObj.transform.position);
        _lineRenderer.SetPosition(1, targetPoint);
    }
    public override void Activate()
    {
        if (!_isActive) TryGrab();
    }
    public override void ForceDeactivate()
    {
        if (_isActive) Release();
    }
    public bool IsSearchingOrGrabbing(GameObject targetObject)
    {
        if (!_isActive || _grabbedObj == null) return false;
        return _grabbedObj.gameObject == targetObject;
    }
    public void ResetOffset()
    {
        if (_grabbedObj == null) return;
        Vector3 tergetPos = _grabbedObj.position + Vector3.up * _liftHeight;
        _relativeOffset = _playerMovement.transform.InverseTransformPoint(tergetPos);
    }
    #endregion
}
