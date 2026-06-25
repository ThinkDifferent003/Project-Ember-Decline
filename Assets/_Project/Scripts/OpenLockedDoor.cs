using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLockedDoor : MonoBehaviour
{
    [SerializeField] private string _requiredItemID;
    [SerializeField] private bool _removeItemOnUse = true;
    [SerializeField] private Animator _animator;
    private bool _isPlayerInRange = false;

    private void Update()
    {
        if (_isPlayerInRange && Input.GetKeyDown(KeyCode.E)) TryOpen();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _isPlayerInRange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) _isPlayerInRange = false;
    }
    public void TryOpen()
    {
        InventoryItem item = InventoryManager.Instance.Items.Find(i => i.Data.ItemID == _requiredItemID);
        if (item != null)
        {
            if (_removeItemOnUse) InventoryManager.Instance.RemoveItem(item.Data, 1);
            _animator?.SetTrigger("Open");
            Debug.Log("Porta Aperta");
        }
        else Debug.Log("Porta Chiusa");
    }
}
 
