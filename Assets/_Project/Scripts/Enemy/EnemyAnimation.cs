using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimation : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (_agent == null || _animator == null) return;
        float currentSpeed = _agent.velocity.magnitude;
        _animator.SetFloat("Speed", currentSpeed);
    }
    public void SetEquippedState(bool equipped)
    {
        _animator?.SetBool("IsEquipped" , equipped);
    }
    public void TriggerAttackAnimation()
    {
        _animator?.SetTrigger("Attack");
    }
    public void TriggerHurt()
    {
        _animator?.ResetTrigger("Hurt");
        _animator?.SetTrigger("Hurt");
    }
    public void SetShieldState(bool isActive)
    {
        _animator?.SetBool("IsShieldActive", isActive);
    }
}
