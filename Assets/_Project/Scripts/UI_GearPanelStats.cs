using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GearPanelStats : MonoBehaviour
{
    public static UI_GearPanelStats Instance;

    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _txtGearName;
    [SerializeField] private TextMeshProUGUI _txtGearModifiers;
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    private int _currentSlotIndex = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (_panel != null) _panel.SetActive(false);
    }
    public void OpenPanel(int slotIndex, GearData data)
    {
        if (data == null) return;
        _currentSlotIndex = slotIndex;
        _panel.SetActive(true);
        _txtGearName.text = data.ItemName;
        _icon.sprite = data.ItemSprite;
        _icon.enabled = true;
        string modifierText = "";
        foreach (var mod in data.Modifier)
        {
            string sign = mod.Value >= 0 ? "+" : "";
            string type = mod.Modifier == ModifierType.Flat ? "" : "%";
            modifierText += $"{mod.Type}: {sign}{mod.Value}{type}\n";
        }
        _txtGearModifiers.text = modifierText;
    }
    public void UnequipButton()
    {
        if (_currentSlotIndex == -1 || EquipmentManager.Instance == null) return;
        EquipmentManager.Instance.UnequipGear(_currentSlotIndex);
        ClosePanel();
    }
    public void ClosePanel()
    {
        _currentSlotIndex = -1;
        if (_panel != null) _panel.SetActive(false);
    }
}
