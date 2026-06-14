using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_WeaponSmith : MonoBehaviour
{
    [SerializeField] private string _mineralID = "Mineral_Iron";
    [SerializeField] private int _mineralCost = 1;
    [SerializeField] private int _increaseDmg = 10;

    public void Interact(GameObject player)
    {
        Debug.Log("[FABBRO] Salve! Porti del materiale da forgiare?");
        PlayerWeaponHandler weaponHandler = player.GetComponent<PlayerWeaponHandler>();
        if (weaponHandler == null || weaponHandler.GetWeaponData() == null) return;
        InventoryItem mineralItem = InventoryManager.Instance.Items.Find(i => i.Data.ItemID == _mineralID);
        if (mineralItem != null && mineralItem.Count >= _mineralCost)
        {
            InventoryManager.Instance.RemoveItem(mineralItem.Data,_mineralCost);
            Debug.Log($"[FABBRO] Ottimo ferro! Rimossi {_mineralCost}x {_mineralID} dal tuo inventario.");
            weaponHandler.EnhancementWeapon(_increaseDmg);
            InventoryManager.Instance.RemoveItem(weaponHandler.GetWeaponData(), 0);
            if (UI_PlayerStats.Instance != null) UI_PlayerStats.Instance.UpdatePanelStats();
            Debug.Log($"[FABBRO] Ecco a te! La tua arma è diventata: {weaponHandler.GetWeaponData().ItemName}!");
        }
        else
        {
            int possessed = mineralItem != null ? mineralItem.Count : 0;
            Debug.LogWarning($"[FABBRO] Non hai abbastanza materiali! Richiesti: {_mineralCost} (Posseduti: {possessed}).");
        }
    }
}
