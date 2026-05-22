using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 720f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;
    private Vector3 _moveInput;
    private Vector3 _moveVelocity;
    private bool _isDashing;
    private float _dashTimeLeft;
    private float _dashCooldownTimer;
    private Vector3 _dashDirection;
    private CharacterController _controller;
    private ProstheticPower[] _prostheticPowers;
    private PlayerHealth _playerHealth;
    private PlayerData _playerData;
    private float _fallBackMove = 5f;
    private float _fallBackDash = 10f;
    public bool IsMoving => _moveInput.magnitude > 0.1f && !_isDashing && (_playerHealth == null || !_playerHealth.IsStunned);

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _prostheticPowers = GetComponentInChildren<Transform>().GetComponentsInChildren<ProstheticPower>(true);
        _playerHealth = GetComponent<PlayerHealth>();
    }
    public void Inizialize(PlayerData data)
    {
        _playerData = data;
    }
    private void Update()
    {
        if (_dashCooldownTimer > 0) _dashCooldownTimer -= Time.deltaTime;
        if (_playerHealth != null && _playerHealth.IsStunned)
        {
            _isDashing = false;
            _moveInput = Vector3.zero;
            _moveVelocity = Vector3.zero;
            _moveVelocity.y = Physics.gravity.y * Time.deltaTime;
            _controller.Move(_moveVelocity * Time.deltaTime);
            return;
        }
        if (_isDashing)
        {
            UpdateDash();
            return;
        }
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        _moveInput = new Vector3(h, 0f, v).normalized;
        bool isAnyPowerActive = false;
        if (_prostheticPowers != null)
        {
            foreach (var power in _prostheticPowers)
            {
                if (power.IsActive)
                {
                    isAnyPowerActive = true;
                    break;
                }
            }
        }
        if (isAnyPowerActive) _moveVelocity = Vector3.zero;
        else if (_moveInput.magnitude > 0.1f)
        {
            float currentSpeed = _playerData != null ? _playerData.MoveSpeed : _fallBackMove;
            _moveVelocity = _moveInput * currentSpeed;
            Quaternion targetRot = Quaternion.LookRotation(_moveInput);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
        }
        else _moveVelocity = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashCooldownTimer <= 0 && _moveInput.magnitude > 0.1f && !isAnyPowerActive) StartDash();
        if (!_isDashing)
        {
            _moveVelocity.y = Physics.gravity.y * Time.deltaTime;
            _controller.Move(_moveVelocity *  Time.deltaTime);
        }
    }
    private void StartDash()
    {
        _isDashing = true;
        _dashTimeLeft = _dashDuration;
        _dashCooldownTimer = _dashCooldown;
        _dashDirection = _moveInput;
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
            
        }
    }
}
