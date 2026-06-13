using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    private List<SavedItem> _savedInventoryItems = new List<SavedItem>();
    public List<SavedItem> SavedInventoryItems => _savedInventoryItems;
}
