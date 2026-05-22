using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;

    private void Awake()
    {
        if (_playerData != null) return;
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null) health.Inizialize(_playerData);
        Movement movement = GetComponent<Movement>();
        if (movement != null) movement.Inizialize(_playerData);
    }
}
