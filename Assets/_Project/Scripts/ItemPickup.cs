using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private int _amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_itemData != null)
            {
                InventoryManager.Instance.AddItem(_itemData, _amount);
                Debug.Log($"[PICKUP] Raccolto: {_itemData.ItemName} x{_amount} aggiunto all'inventario.");
                Destroy(gameObject);
            }
        }
    }
}
