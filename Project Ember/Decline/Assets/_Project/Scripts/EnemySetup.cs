using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySetup : MonoBehaviour
{
    [SerializeField] private EnemyData _stats;
    public EnemyData Stats => _stats;

    private void Awake()
    {
        if (_stats == null) return;
        SetupHealth();
        SetupAI();
    }
    private void SetupHealth()
    {
        EnemyHealth health = GetComponent<EnemyHealth>();
        if (health != null) health.Inizialize(_stats);
    }
    private void SetupAI()
    {
        EnemyAI ai = GetComponent<EnemyAI>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (ai != null) ai.Inizialize(_stats);
        if (agent != null) agent.speed = _stats.MoveSpeed;
    }
}
