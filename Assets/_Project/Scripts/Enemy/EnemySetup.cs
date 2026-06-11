using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySetup : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private EnemyData _stats;
    [SerializeField] private int _enemyLevel = 1;
    #region - Public Proprierties -
    public EnemyData Stats => _stats;
    public int EnemyLevel => _enemyLevel;
    #endregion

    #region - Lyfe Cycle -
    private void Awake()
    {
        if (_stats == null) return;
        if (_enemyLevel < 1) _enemyLevel = 1;
        SetupHealth();
        SetupAI();
    }
    #endregion
    #region - Core Logic -
    private void SetupHealth()
    {
        EnemyHealth health = GetComponent<EnemyHealth>();
        health?.Inizialize(_stats , _enemyLevel);
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
