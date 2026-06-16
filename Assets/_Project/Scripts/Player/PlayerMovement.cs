using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _rotationSpeed = 720f;
    [SerializeField] private float _fallBackMove = 5f;
    [Header("Dash Setiings")]
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;
    [SerializeField] private float _fallBackDash = 10f;
    [SerializeField] private float _dashCost = 20f;
    [Header("Components")]
    [SerializeField] private PlayerLockOn _lockOn;
    [SerializeField] private CinemachineFreeLook _cam;
    private CharacterController _controller;
    private Transform _camTransform;
    private PowersManager _powersManager;
    private PlayerHealth _playerHealth;
    private PlayerData _playerData;
    private PlayerStamina _playerStamina;

    private Vector3 _moveInput;
    private Vector3 _moveVelocity;
    private Vector3 _dashDirection;
    
    private bool _isDashing;
    private float _dashTimeLeft;
    private float _dashCooldownTimer;
    private float _verticalVelocity;
    private bool _canMove = true;
    private float _originalXSpeed;
    private float _originalYSpeed;
    public bool IsMoving => _moveInput.magnitude > 0.1f && !_isDashing && (_playerHealth == null || !_playerHealth.IsStunned);

    #region - Lyfe Cycle -
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _powersManager = GetComponent<PowersManager>();
        _playerHealth = GetComponent<PlayerHealth>();
        _playerStamina = GetComponent<PlayerStamina>();
        if (Camera.main != null) _camTransform = Camera.main.transform;
        if (_cam != null)
        {
            _originalXSpeed = _cam.m_XAxis.m_MaxSpeed;
            _originalYSpeed = _cam.m_YAxis.m_MaxSpeed;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        if (Time.timeScale == 0) return;
        if (!_canMove) return;
        HandleTimers();
        HandleGravity();
        if (_playerHealth != null && _playerHealth.IsStunned)
        {
            ApplyStunMovement();
            return;
        }
        if (_isDashing)
        {
            UpdateDash();
            return;
        }
        HandleInput();
        HandleMovement();
        if (!_isDashing)
        {
            _moveVelocity.y = _verticalVelocity;
            _controller.Move(_moveVelocity * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.T)) _lockOn.ToggleLock();
    }
    private void OnEnable()
    {
        DialogueManager.OnDialogueStart += BlockMove;
        DialogueManager.OnDialogueEnd += NoBlockMove;
    }
    private void OnDisable()
    {
        DialogueManager.OnDialogueStart -= BlockMove;
        DialogueManager.OnDialogueEnd -= NoBlockMove;
    }
    #endregion
    #region - Core Logic -
    private void HandleTimers()
    {
        if (_dashCooldownTimer > 0) _dashCooldownTimer -= Time.deltaTime;
    }
    private void HandleGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0) _verticalVelocity = -2f;
        else _verticalVelocity += Physics.gravity.y * Time.deltaTime;
    }
    private void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (_lockOn != null && _lockOn.IsLocked)
        {
            Vector3 targetDir = (_lockOn.CurrentTarget.position - transform.position).normalized;
            targetDir.y = 0;
            Vector3 right = Vector3.Cross(Vector3.up, targetDir);
            _moveInput = (targetDir * v + right * h).normalized;
        }
        else if (_camTransform != null && (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f))
        {
            Vector3 camForward = Vector3.Scale(_camTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(_camTransform.right, new Vector3(1, 0, 1)).normalized;
            _moveInput = (camForward * v + camRight * h).normalized;
        }
        else _moveInput = Vector3.zero;
    }
    private void HandleMovement()
    {
        bool isAnyPowerActive = _powersManager != null && _powersManager.GetCurrentSelectedPower()?.IsActive == true;
        if (isAnyPowerActive) _moveVelocity = Vector3.zero;
        else if (_moveInput.magnitude > 0.1f)
        {
            float currentSpeed = _playerData != null ? _playerData.MoveSpeed : _fallBackMove;
            _moveVelocity = _moveInput * currentSpeed;
            if (_lockOn != null && _lockOn.IsLocked)
            {
                Vector3 targetDir = (_lockOn.CurrentTarget.position - transform.position).normalized;
                targetDir.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
            else
            {
                Quaternion targetRot = Quaternion.LookRotation(_moveInput);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
            }
        }
                
            
        else _moveVelocity = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashCooldownTimer <= 0 && _moveInput.magnitude > 0.1f && !isAnyPowerActive)
        {
            if (_playerStamina != null && _playerStamina.TryConsumeStamina(_dashCost)) StartDash();
        }  
    }
    #endregion
    #region - Dash Logic _
    private void StartDash()
    {
        _isDashing = true;
        _dashTimeLeft = _dashDuration;
        _dashCooldownTimer = _dashCooldown;
        _dashDirection = _moveInput;
        _verticalVelocity = 0f;
        if (_playerStamina != null) _playerStamina.SetRegenerationBlocked(true);
    }
    private void UpdateDash()
    {
        if (_dashTimeLeft > 0f)
        {
            float currentSpeed = _playerData != null ? _playerData.DashSpeed : _fallBackDash;
            _controller.Move(_dashDirection * currentSpeed * Time.deltaTime);
            _dashTimeLeft -= Time.deltaTime;
        }
        else
        {
            _isDashing = false;
            if (_playerStamina != null) _playerStamina.SetRegenerationBlocked(false);
        }     
    }
    #endregion
    #region _ Utility _
    public void Inizialize(PlayerData data)
    {
        _playerData = data;
    }
    private void ApplyStunMovement()
    {
        _isDashing = false;
        if (_playerStamina != null) _playerStamina.SetRegenerationBlocked(false);
        _moveInput = Vector3.zero;
        _moveVelocity = Vector3.zero;
        _moveVelocity.y = _verticalVelocity;
        _controller.Move(_moveVelocity * Time.deltaTime);
    }
    private void BlockMove()
    {
        _canMove = false;
        _moveInput = Vector3.zero;
        _moveVelocity = Vector3.zero;
        if (_cam != null)
        {
            _cam.m_XAxis.m_MaxSpeed = 0f;
            _cam.m_YAxis.m_MaxSpeed = 0f;
        }
    }
    private void NoBlockMove()
    {
        _canMove = true;
        if (_cam != null)
        {
            _cam.m_XAxis.m_MaxSpeed = _originalXSpeed;
            _cam.m_YAxis.m_MaxSpeed = _originalYSpeed;
        }
    }
    #endregion
}
