using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    private List<SavedItem> _savedItems = new List<SavedItem>();
    public List<SavedItem> SavedItems => _savedItems;
}
