using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySetup : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private EnemyData _stats;
    #region - Public Proprierties -
    public EnemyData Stats => _stats;
    #endregion

    #region - Lyfe Cycle -
    private void Awake()
    {
        if (_stats == null) return;
        SetupHealth();
        SetupAI();
    }
    #endregion
    #region - Core Logic -
    private void SetupHealth()
    {
        EnemyHealth health = GetComponent<EnemyHealth>();
        health?.Inizialize(_stats);
    }
    private void SetupAI()
    {
        EnemyAI ai = GetComponent<EnemyAI>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        ai?.Inizialize(_stats);
        if (agent != null) agent.speed = _stats.MoveSpeed;
    }
    #endregion
}
