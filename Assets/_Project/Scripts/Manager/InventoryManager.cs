using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private List<InventoryItem> _items = new List<InventoryItem>();
    [SerializeField] private List<ItemData> _allItemsData = new List<ItemData>();
    public List<InventoryItem> Items => _items;
    public static event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void AddItem(ItemData itemData , int amount = 1)
    {
        if (itemData.IsStackable)
        {
            InventoryItem existingItem = _items.Find(i => i.Data.ItemID == itemData.ItemID);
            if (existingItem != null) 
            {
                existingItem.Count += amount;
                OnInventoryChanged?.Invoke();
                return;
            }
        }
        _items.Add(new InventoryItem(itemData , amount));
        OnInventoryChanged?.Invoke();
    }
    public void RemoveItem(ItemData itemData , int amount = 1)
    {
        InventoryItem existingItem = _items.Find(i => i.Data.ItemID == itemData.ItemID);
        if (existingItem != null)
        {
            if (itemData.IsStackable)
            {
                existingItem.Count -= amount;
                if (existingItem.Count <= 0) _items.Remove(existingItem);
            }
            else _items.Remove(existingItem);
            OnInventoryChanged?.Invoke();
        }
    }
    public void PopulateSaveData(GameSaveData saveData)
    {
        foreach (InventoryItem item in _items)
        {
            SavedItem savedItem = new SavedItem { ID = item.Data.ItemID, Count = item.Count };
            saveData.SavedInventoryItems.Add(savedItem);
        }
    }
    public void LoadFromSaveData(GameSaveData saveData)
    {
        _items.Clear();
        foreach (SavedItem savedItem in saveData.SavedInventoryItems)
        {
            ItemData foundItem = _allItemsData.Find(item => item.ItemID == savedItem.ID);
            if (foundItem != null)
            {
                ItemData runtimeItem = ScriptableObject.Instantiate(foundItem);
                _items.Add(new InventoryItem(foundItem, savedItem.Count));
            }
            else Debug.LogWarning($"[InventoryManager] Errore: ID '{savedItem.ID}' non trovato nel database durante il caricamento!");
        }
        OnInventoryChanged?.Invoke();
    }
}
