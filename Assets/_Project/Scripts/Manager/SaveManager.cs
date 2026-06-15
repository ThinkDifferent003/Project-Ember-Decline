using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string _savePath;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        _savePath = Path.Combine(Application.persistentDataPath, "game_save.json");
    }
    private void Start()
    {
        LoadGame();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("[SaveManager] Premuto F5: Avvio salvataggio rapido...");
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Debug.Log("[SaveManager] Premuto F9: Avvio caricamento rapido...");
            LoadGame();
        }
    }
    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();
        if (InventoryManager.Instance != null)
        {
            Debug.Log($"[DEBUG SALVATAGGIO] Oggetti in inventario prima del popolamento: {InventoryManager.Instance.Items.Count}");
            InventoryManager.Instance.PopulateSaveData(saveData);
            Debug.Log($"[DEBUG SALVATAGGIO] Oggetti impacchettati nel saveData: {saveData.SavedInventoryItems.Count}");
        }
        else Debug.LogError("[DEBUG SALVATAGGIO] Errore: InventoryManager.Instance è NULL!");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerWeaponHandler weaponHandler = player.GetComponent<PlayerWeaponHandler>();
            if (weaponHandler != null && weaponHandler.GetWeaponData()) 
            {
                WeaponData data = weaponHandler.GetWeaponData();
                saveData.EquippedWeaponID = data.ItemID;
                saveData.WeaponLevel = data.Level;
                saveData.WeaponDamage = data.Damage;
                Debug.Log($"[SaveManager] Salvati dati arma: {data.ItemName} (+{data.Level})");
            }
        }
        PlayerLeveling leveling = player.GetComponent<PlayerLeveling>();
        if (leveling != null)
        {
            saveData.PlayerLevel = leveling.CurrentLevel;
            saveData.PlayerCurrentXp = leveling.CurrentXp;
        }
        if (EquipmentManager.Instance != null)
        {
            for (int i = 0; i < 3; i++)
            {
                GearData gear = EquipmentManager.Instance.GetGearSlot(i);
                if (gear != null) saveData.EquippedGearsID[i] = gear.ItemID;
                else saveData.EquippedGearsID[i] = "";
            }
        }
        string json = JsonUtility.ToJson(saveData, true);
        Debug.Log($"[DEBUG SALVATAGGIO] Stringa JSON generata:\n{json}");
        File.WriteAllText(_savePath, json);
        Debug.Log($"[SaveManager] Gioco salvato con successo in: {_savePath}");
    }
    public void LoadGame()
    {
        if (!File.Exists(_savePath))
        {
            Debug.Log("[SaveManager] Nessun file di salvataggio trovato. Inizio nuova partita.");
            return;
        }
        string json = File.ReadAllText(_savePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
        if (InventoryManager.Instance != null) InventoryManager.Instance.LoadFromSaveData(saveData);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerWeaponHandler weaponHandler = player.GetComponent<PlayerWeaponHandler>();
            if (weaponHandler != null) weaponHandler.UpdateSavedWeapon(saveData.WeaponLevel, saveData.WeaponDamage);
            PlayerLeveling leveling = player.GetComponent<PlayerLeveling>();
            if (leveling != null)
            {
                int levelToLoad = saveData.PlayerLevel > 0 ? saveData.PlayerLevel : 1;
                leveling.LoadLevelData(levelToLoad, saveData.PlayerCurrentXp);
            }
        }
        if (EquipmentManager.Instance != null && saveData.EquippedGearsID != null)
        {
            EquipmentManager.Instance.ClearAllGears();
            for (int i = 0; i < saveData.EquippedGearsID.Length; i++)
            {
                string id = saveData.EquippedGearsID[i];
                if (string.IsNullOrEmpty(id)) continue;
                GearData gearData = InventoryManager.Instance.GetItemDataByID(id) as GearData;
                if (gearData != null)
                {
                    GearData runtimeGear = ScriptableObject.Instantiate(gearData);
                    EquipmentManager.Instance.EquipFromSave(runtimeGear, i);
                }
            }
            EquipmentManager.Instance.ApplyGearModifiers();
        }
        if (UI_PlayerStats.Instance != null) UI_PlayerStats.Instance.UpdatePanelStats();
        Debug.Log("[SaveManager] Gioco caricato con successo!");
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
