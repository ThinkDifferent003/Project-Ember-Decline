using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_PlayerStats : MonoBehaviour
{
    public static UI_PlayerStats Instance;
    [SerializeField] private PlayerLeveling _playerLeveling;
    [SerializeField] private PlayerWeaponHandler _weaponHandler;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private GameObject _statsPanel;
    [SerializeField] private GameObject _weaponPanel;
    [SerializeField] private TextMeshProUGUI _txtLevel;
    [SerializeField] private TextMeshProUGUI _txtMaxHealth;
    [SerializeField] private TextMeshProUGUI _txtExp;
    [SerializeField] private TextMeshProUGUI _txtDef;
    [SerializeField] private TextMeshProUGUI _txtMultiplier;
    [SerializeField] private TextMeshProUGUI _txtWeaponName;
    [SerializeField] private TextMeshProUGUI _txtWeaponDmg;
    [SerializeField] private TextMeshProUGUI _txtWeaponLevel;
    [SerializeField] private TextMeshProUGUI _txtTotalAttack;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        if (_statsPanel != null) _statsPanel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) ToggleStatsPanel();
    }
    private void OnEnable()
    {
        UpdatePanelStats();
    }
    private void ToggleStatsPanel()
    {
        if (_statsPanel == null) return;
        bool isActive = !_statsPanel.activeSelf;
        _statsPanel.SetActive(isActive);
        if (isActive)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            UpdatePanelStats();
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (_weaponPanel != null) _weaponPanel.SetActive(false);
        }
    }
    public void UpdatePanelStats()
    {
        if (_playerLeveling == null || _weaponHandler == null) return;
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
        _txtLevel.text = $"Livello: {currentLevel}";
        _txtMaxHealth.text = $"PV Massimi: {_playerHealth.MaxHealth}";
        _txtExp.text = $"XP: {Mathf.RoundToInt(_playerLeveling.CurrentXp)} / {Mathf.RoundToInt(_playerLeveling.XpRequested)}";
        _txtDef.text = $"Difesa Totale: {_playerLeveling.CurrentDefense}";
        _txtMultiplier.text = $"Bonus Forza: x{multiplier:F2}";
    }
    
}
