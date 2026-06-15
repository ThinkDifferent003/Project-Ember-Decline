using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerStamina _playerStamina;
    [SerializeField] private PlayerLeveling _playerLeveling;
    [SerializeField] private PlayerSetup _playerSetup;
    private PlayerData _playerData;
    private GearData[] _gears = new GearData[3];
    public event Action OnGearEquipmentChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        if (_playerSetup != null) _playerData = _playerSetup.PlayerData;
        ApplyGearModifiers();
    }
    public bool EquipGear(GearData newGear)
    {
        if (newGear == null) return false;
        int slotDestination = -1;
        for (int i = 0; i < _gears.Length; i++)
        {
            if (_gears[i] == null)
            {
                slotDestination = i;
                break;
            }
        }
        if (slotDestination == -1)
        {
            slotDestination = 0;
            GearData oldGear = _gears[slotDestination];
            InventoryManager.Instance.AddItem(oldGear);
        }
        _gears[slotDestination] = newGear;
        ApplyGearModifiers();
        OnGearEquipmentChanged?.Invoke();
        return true;
    }
    public void UnequipGear(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _gears.Length) return;
        if (_gears[slotIndex] != null)
        {
            InventoryManager.Instance.AddItem(_gears[slotIndex]);
            _gears[slotIndex] = null;
            ApplyGearModifiers();
            OnGearEquipmentChanged?.Invoke();
        }
    }
    public void ApplyGearModifiers()
    {
        if (_playerData == null && _playerSetup != null) _playerData = _playerSetup.PlayerData;
        if (_playerLeveling == null || _playerData == null)
        {
            Debug.LogError($"[EQUIPMENT ERROR] Impossibile calcolare i bonus! _playerLeveling è null? {_playerLeveling == null} | _playerData è null? {_playerData == null}");
            return;
        }
        float flatHealth = 0f;
        float percentHealth = 0f;
        float flatStamina = 0f;
        float percentStamina = 0f;
        foreach (GearData gear in _gears)
        {
            if (gear == null) continue;
            foreach (var mod in gear.Modifier)
            {
                if (mod.Type == StatType.Health)
                {
                    if (mod.Modifier == ModifierType.Flat) flatHealth += mod.Value;
                    else percentHealth += mod.Value;
                }
                else if (mod.Type == StatType.Stamina)
                {
                    if (mod.Modifier == ModifierType.Flat) flatStamina += mod.Value;
                    else percentStamina += mod.Value;
                }
            }
        }
        int currentLevel = _playerLeveling.CurrentLevel;
        float baseHealth = _playerData.GetMaxHealth(currentLevel);
        float baseStamina = _playerData.GetStamina(currentLevel);

        float finalHealth = (baseHealth + flatHealth) * (1f + percentHealth);
        float finalStamina = (baseStamina + flatStamina) * (1f + percentStamina);
        Debug.Log($"[STATS CALC] Livello Player: {currentLevel} | HP Base: {baseHealth} | Bonus Anello: +{flatHealth} | HP Finali calcolati: {finalHealth}");
        if (_playerHealth != null) _playerHealth.NotifyMaxHealthChanged(finalHealth);
    }
    public GearData GetGearSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < _gears.Length) return _gears[slotIndex];
        return null;
    }
    public void ClearAllGears()
    {
        for (int i = 0; i < _gears.Length; i++) _gears[i] = null;
    }
    public void EquipFromSave(GearData gear, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < _gears.Length) _gears[slotIndex] = gear;
    }
}
