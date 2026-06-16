using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStats : MonoBehaviour
{
    public static UI_PlayerStats Instance;
    [SerializeField] private PlayerLeveling _playerLeveling;
    [SerializeField] private PlayerWeaponHandler _weaponHandler;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerEnergy _playerEnergy;
    [SerializeField] private PlayerStamina _playerStamina;
    [SerializeField] private GameObject _statsPanel;
    [SerializeField] private GameObject _weaponPanel;
    [SerializeField] private TextMeshProUGUI _txtPlayerName;
    [SerializeField] private TextMeshProUGUI _txtLevel;
    [SerializeField] private TextMeshProUGUI _txtMaxHealth;
    [SerializeField] private TextMeshProUGUI _txtExp;
    [SerializeField] private TextMeshProUGUI _txtDef;
    [SerializeField] private TextMeshProUGUI _txtEnergy;
    [SerializeField] private TextMeshProUGUI _txtStamina;
    [SerializeField] private TextMeshProUGUI _txtMultiplier;
    [SerializeField] private TextMeshProUGUI _txtWeaponName;
    [SerializeField] private TextMeshProUGUI _txtWeaponDmg;
    [SerializeField] private TextMeshProUGUI _txtWeaponLevel;
    [SerializeField] private TextMeshProUGUI _txtTotalAttack;
    [SerializeField] private Slider _xpBar;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        if (EquipmentManager.Instance != null) EquipmentManager.Instance.OnGearEquipmentChanged += UpdatePanelStats;
        if (_statsPanel != null) _statsPanel.SetActive(false);
        UpdatePanelStats();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) ToggleStatsPanel();
    }
    private void OnEnable()
    {
        UpdatePanelStats();
    }
    private void OnDestroy()
    {
        if (EquipmentManager.Instance != null) EquipmentManager.Instance.OnGearEquipmentChanged -= UpdatePanelStats;
    }
    public bool IsPanelActive()
    {
        return _statsPanel != null && _statsPanel.activeSelf;
    }
    private void ToggleStatsPanel()
    {
        if (_statsPanel == null) return;
        bool isActive = !_statsPanel.activeSelf;
        _statsPanel.SetActive(isActive);
        if (isActive)
        {
            if (InventoryDisplay.Instance != null) InventoryDisplay.Instance.CloseInventroyForced();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            UpdatePanelStats();
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (_weaponPanel != null) _weaponPanel.SetActive(false);
            if (UI_GearPanelStats.Instance != null) UI_GearPanelStats.Instance.ClosePanel();
            Time.timeScale = 1f;
        }
    }
    public void UpdatePanelStats()
    {
        if (_playerLeveling == null || _weaponHandler == null || _playerHealth == null || _playerEnergy == null || _playerStamina == null) return;
        int currentLevel = _playerLeveling.CurrentLevel;
        float multiplier = _playerLeveling.AttackMultiplier;
        WeaponData weapon = _weaponHandler.GetWeaponData();
        if (weapon != null)
        {
            _txtWeaponName.text = weapon.ItemName;
            _txtWeaponLevel.text = $"Livello Arma: +{weapon.Level}";
            _txtWeaponDmg.text = $"Danno Base: {weapon.Damage}"; 
            int totalAttack = Mathf.RoundToInt(weapon.Damage * multiplier);
            _txtTotalAttack.text = $"POTENZA ATTACCO: {totalAttack}";
        }
        _txtPlayerName.text = _playerLeveling.CharacterName;
        _txtLevel.text = $"LV: {currentLevel}";
        _txtMaxHealth.text = $"HP: {_playerHealth.MaxHealth}";
        _txtEnergy.text = $"EN: {_playerEnergy.MaxEnergy}";
        _txtStamina.text = $"ST: {_playerStamina.MaxStamina}";
        _txtExp.text = $"XP: {Mathf.RoundToInt(_playerLeveling.CurrentXp)} / {Mathf.RoundToInt(_playerLeveling.XpRequested)}";
        _txtDef.text = $"DF: {_playerLeveling.CurrentDefense}";
        _txtMultiplier.text = $"FR: x{multiplier:F2}";
        _xpBar.maxValue = _playerLeveling.XpRequested;
        _xpBar.value = _playerLeveling.CurrentXp;
    }
    public void ClosePanelForced()
    {
        if (_statsPanel != null && _statsPanel.activeSelf)
        {
            _statsPanel.SetActive(false);
            if (_weaponPanel != null) _weaponPanel.SetActive(false);
            if (UI_GearPanelStats.Instance != null) UI_GearPanelStats.Instance.ClosePanel();
        }
    }
}
