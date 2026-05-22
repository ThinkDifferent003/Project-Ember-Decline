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
        if (_animator != null) _animator.SetBool("IsEquipped" , equipped);
    }
    public void TriggerAttackAnimation()
    {
        if (_animator != null) _animator.SetTrigger("Attack");
    }
}
