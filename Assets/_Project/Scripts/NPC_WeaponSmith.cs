using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_WeaponSmith : NPC_Manager
{
    [SerializeField] private string _mineralID = "Mineral_Iron";
    [SerializeField] private int _increaseDmg = 10;
    private PlayerWeaponHandler _cachedWeaponHandler;

    private void OnEnable()
    {
        DialogueManager.OnDialogueEventTriggered += HandleSmithEvents;
    }
    private void OnDisable()
    {
        DialogueManager.OnDialogueEventTriggered -= HandleSmithEvents;
    }
    public override void Interact(GameObject player)
    {
        Debug.Log("[FABBRO] Inizio interazione e passaggio dati a Ink... ");
        _cachedWeaponHandler = player.GetComponent<PlayerWeaponHandler>();
        if (_cachedWeaponHandler == null || _cachedWeaponHandler.GetWeaponData() == null) return;
        InventoryItem mineralItem = InventoryManager.Instance.Items.Find(i => i.Data.ItemID == _mineralID);
        int minerals = mineralItem != null ? mineralItem.Count : 0;
        int weaponLevel = _cachedWeaponHandler.GetWeaponData().Level;
        if (weaponLevel <= 0) weaponLevel = 1;
        DialogueManager.Instance.SetInkVariable("Minerali_Giocatore", minerals);
        DialogueManager.Instance.SetInkVariable("Livello_Spada", weaponLevel);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        base.Interact(player);
        
    }
    private void HandleSmithEvents(string eventName)
    {
        if (eventName.StartsWith("UpgradeSpada_"))
        {
            string livelloStr = eventName.Replace("UpgradeSpada_", "");
            int newLevel = int.Parse(livelloStr);
            if (_cachedWeaponHandler != null)
            {
                _cachedWeaponHandler.EnhancementWeapon(_increaseDmg);
                InventoryManager.Instance.RemoveItem(_cachedWeaponHandler.GetWeaponData(), 0);
                if (UI_PlayerStats.Instance != null) UI_PlayerStats.Instance.UpdatePanelStats();
                Debug.Log($"[FABBRO UNITY] Spada potenziata! Nuovo livello: {newLevel}. Danno aumentato di {_increaseDmg}.");
            }
        }
        if (eventName.StartsWith("SincronizzaMinerali_"))
        {
            string mineralStr = eventName.Replace("SincronizzaMinerali_", "");
            int inkMineral = int.Parse(mineralStr);
            InventoryItem mineralItem = InventoryManager.Instance.Items.Find(i => i.Data.ItemID == _mineralID);
            if (mineralItem != null)
            {
                int mineralSpent = mineralItem.Count - inkMineral;
                if (mineralSpent > 0)
                {
                    InventoryManager.Instance.RemoveItem(mineralItem.Data, mineralSpent);
                    Debug.Log($"[FABBRO UNITY] Rimossi {mineralSpent}x {_mineralID} dall'inventario di Unity.");

                }
            }
        }
    }
}
