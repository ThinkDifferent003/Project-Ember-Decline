using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerData _playerData;
    #region - Public Proprierties -
    public PlayerData PlayerData => _playerData;
    #endregion

    #region - Lyfe Cycle
    private void Awake()
    {
        InizializeComponents();
    }
    #endregion
    #region - Core Logic -
    private void InizializeComponents()
    {
        if (_playerData == null) return;
        PlayerHealth health = GetComponent<PlayerHealth>();
        health?.Inizialize(_playerData);
        PlayerMovement movement = GetComponent<PlayerMovement>();
        movement?.Inizialize(_playerData);
    }
    #endregion
}
