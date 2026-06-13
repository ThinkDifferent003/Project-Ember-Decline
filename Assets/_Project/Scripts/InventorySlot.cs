using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _countText;
    private InventoryItem _currentSlotItem;

    public void SetupSlot(InventoryItem newItem)
    {
        _currentSlotItem = newItem;
        _itemIcon.sprite = _currentSlotItem.Data.ItemSprite;
        _itemIcon.enabled = true;
        _itemIcon.gameObject.SetActive(true);
        if (_currentSlotItem.Data.IsStackable && _currentSlotItem.Count > 1)
        {
            _countText.text = _currentSlotItem.Count.ToString();
            _countText.gameObject.SetActive(true);
        }
        else _countText.gameObject.SetActive(false);
    }
    public void ClearSlot()
    {
        _currentSlotItem = null;
        _itemIcon.sprite = null;
        _itemIcon.enabled = false;
        _countText.gameObject.SetActive(false);
    }
    public void OnSlotClicked()
    {
        if (_currentSlotItem != null) InventoryDisplay.Instance.ShowDesciption(_currentSlotItem.Data);
    }
}
