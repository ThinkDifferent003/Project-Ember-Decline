using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int _slotIndex = 0;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Sprite _sprite;

    private void Start()
    {
        if (EquipmentManager.Instance != null) EquipmentManager.Instance.OnGearEquipmentChanged += RefreshSlotUI;
        RefreshSlotUI();
    }
    private void OnDestroy()
    {
        if (EquipmentManager.Instance != null) EquipmentManager.Instance.OnGearEquipmentChanged -= RefreshSlotUI;
    }
    public void RefreshSlotUI()
    {
        if (EquipmentManager.Instance == null || _iconImage == null) return;
        GearData gearData = EquipmentManager.Instance.GetGearSlot(_slotIndex);
        if (gearData != null) 
        {
            _iconImage.sprite = gearData.ItemSprite;
            _iconImage.enabled = true;
        }
        else
        {
            if (_sprite != null)
            {
                _iconImage.sprite = _sprite;
                _iconImage.enabled = true;
            }
            else _iconImage.enabled = false;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (EquipmentManager.Instance == null) return;
        GearData currentGear = EquipmentManager.Instance.GetGearSlot(_slotIndex);
        if (currentGear != null && UI_GearPanelStats.Instance != null) UI_GearPanelStats.Instance.OpenPanel(_slotIndex, currentGear); 
    }
    public void UnequipButton()
    {
        if (EquipmentManager.Instance != null) EquipmentManager.Instance.UnequipGear(_slotIndex);
    }
}
