using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    public static InventoryDisplay Instance;

    [SerializeField] private Transform _gridParent;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private GameObject _descritionPanel;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private Image _selectedIcon;
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private KeyCode _toggleKey = KeyCode.I;
    [SerializeField] private GameObject _equipButton;

    private ItemData _currentItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        _inventoryPanel.SetActive(false);
        _descritionPanel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(_toggleKey)) ToggleInventory();
    }
    private void OnEnable()
    {
        InventoryManager.OnInventoryChanged += UpdateUI;
    }
    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= UpdateUI;
    }
    public void ToggleInventory()
    {
        bool isActive = !_inventoryPanel.activeSelf;
        _inventoryPanel.SetActive(isActive);
        if (isActive)
        {
            UpdateUI();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            _descritionPanel.SetActive(false);
            _currentItem = null;
            Cursor.lockState= CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void UpdateUI()
    {
        foreach (Transform child in _gridParent) Destroy(child.gameObject);
        foreach (InventoryItem item in InventoryManager.Instance.Items)
        {
            GameObject newSlot = Instantiate(_slotPrefab, _gridParent);
            newSlot.GetComponent<InventorySlot>().SetupSlot(item);
        }
    }
    public void ShowDesciption(ItemData item)
    {
        _currentItem = item;
        _descritionPanel.SetActive(true);
        _itemNameText.text = item.ItemName;
        _itemDescriptionText.text = item.Description;
        if (item.ItemSprite != null)
        {
            _selectedIcon.sprite = item.ItemSprite;
            _selectedIcon.enabled = true;
        }
        else _selectedIcon.enabled = false;
        if (_equipButton != null) _equipButton.SetActive(item is GearData); 
    }
    public void EquipButton()
    {
        Debug.Log("[DEBUG UI] Bottone Equipaggia premuto!");
        if (_currentItem == null)
        {
            Debug.LogError("[DEBUG UI] Errore: _currentItem è NULL!");
            return;
        } 
        GearData gearToEquip = _currentItem as GearData;
        if (gearToEquip == null)
        {
            Debug.LogError($"[DEBUG UI] Errore: {_currentItem.ItemName} NON è un GearData valido!");
            return;
        }
        if (EquipmentManager.Instance == null)
        {
            Debug.LogError("[DEBUG UI] Errore: EquipmentManager.Instance è NULL nella scena!");
            return;
        }

        Debug.Log($"[DEBUG UI] Invio {gearToEquip.ItemName} all'EquipmentManager...");

        bool hasEquipped = EquipmentManager.Instance.EquipGear(gearToEquip);
        if (hasEquipped)
        {
            InventoryManager.Instance.RemoveItem(_currentItem, 1);
            if (UI_PlayerStats.Instance != null) UI_PlayerStats.Instance.UpdatePanelStats();
            _descritionPanel.SetActive(false);
            _currentItem = null;
        }
        else Debug.LogWarning("[DEBUG UI] EquipmentManager ha rifiutato l'oggetto (forse EquipGear ha restituito false).");
    }
}
