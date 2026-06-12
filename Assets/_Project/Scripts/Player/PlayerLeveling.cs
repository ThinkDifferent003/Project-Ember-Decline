using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLeveling : MonoBehaviour
{
    [SerializeField] private AnimationCurve _xpRequestedCurve;
    [SerializeField] private int _currentLevel = 1;
    [SerializeField] private int _maxLevel = 30;
    private float _currentXp = 0f;
    private PlayerHealth _playerHealth;
    private PlayerEnergy _playerEnergy;
    private PlayerStamina _playerStamina;
    private PlayerData _playerData;

    public int CurrentLevel => _currentLevel;
    public float CurrentXp => _currentXp;
    public float XpRequested => _xpRequestedCurve.Evaluate(_currentLevel);
    public float AttackMultiplier => _playerData != null ? _playerData.GetAttackMultiplier(_currentLevel) : 1f;
    public float CurrentDefense => _playerData != null ? _playerData.GetDefense(_currentLevel) : 0f;

    private void Start()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        _playerEnergy = GetComponent<PlayerEnergy>();
        _playerStamina = GetComponent<PlayerStamina>();
        PlayerSetup setup = GetComponent<PlayerSetup>();
        if (setup != null) _playerData = setup.PlayerData;
        if (_playerHealth != null) _playerHealth.UpdateHealthOnLevelUp(_currentLevel);
        PrintStats();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            Debug.Log(" [TEST] Premuto L: Aggiungo 50 XP!");
            AddXp(50f);
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            Debug.Log(" [TEST] Premuto K: Forzo Level Up istantaneo!");
            ForceLevelUp();
        }
    }
    private void LevelUp()
    {
        _currentLevel++;
        if (_playerHealth != null) _playerHealth.UpdateHealthOnLevelUp(_currentLevel);
        if (_playerEnergy != null) _playerEnergy.UpdateEnergyStats();
        if (_playerStamina != null) _playerStamina.UpdateStaminaStats();
        Debug.LogWarning($"⭐ LEVEL UP! Sei passato al Livello {_currentLevel}! ⭐");
        PrintStats();
    }
    public void PrintStats()
    {
        float hpMaxAttuali = _playerHealth != null ? _playerHealth.MaxHealth : 0f;

        Debug.Log($"=== REQUISITI PLAYER (LIV. {_currentLevel}) ===\n" +
                  $"- HP Massimi attuali: {hpMaxAttuali}\n" +
                  $"- Moltiplicatore Attacco: {AttackMultiplier}x\n" +
                  $"- Difesa Attuale: {CurrentDefense}\n" +
                  $"- XP Correnti: {_currentXp} / {XpRequested} (per il prossimo livello)");
    }
    public void AddXp(float xp)
    {
        if (_currentLevel >= _maxLevel) return;
        _currentXp += xp;
        Debug.Log($"Guadagnati {xp} XP. Totale: {_currentXp}/{XpRequested}");
        while (_currentXp >= XpRequested && _currentLevel < _maxLevel)
        {
            _currentXp -= XpRequested;
            LevelUp();
        }
    }
    public void ForceLevelUp()
    {
        if (_currentLevel < _maxLevel)
        {
            _currentXp = 0f;
            LevelUp();
        }
    }
}

