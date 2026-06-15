using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    [SerializeField] private List<SavedItem> _savedInventoryItems = new List<SavedItem>();
    public List<SavedItem> SavedInventoryItems => _savedInventoryItems;

    public string[] EquippedGearsID = new string[3];
    public string EquippedWeaponID;
    public int WeaponLevel;
    public float WeaponDamage;
    public int PlayerLevel;
    public float PlayerCurrentXp;
}
