using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();
        if (InventoryManager.Instance != null) InventoryManager.Instance.PopulateSaveData(saveData);
        string json = JsonUtility.ToJson(saveData,true);
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
        Debug.Log("[SaveManager] Gioco caricato con successo!");
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
