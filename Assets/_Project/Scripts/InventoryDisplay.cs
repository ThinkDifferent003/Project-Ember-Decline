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
        _descritionPanel.SetActive(true);
        _itemNameText.text = item.ItemName;
        _itemDescriptionText.text = item.Description;
        if (item.ItemSprite != null)
        {
            _selectedIcon.sprite = item.ItemSprite;
            _selectedIcon.enabled = true;
        }
        else _selectedIcon.enabled = false;
    }
}
