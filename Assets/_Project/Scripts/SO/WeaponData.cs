using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon" , menuName = "Weapons")]
public class WeaponData : ItemData
{
    [Header("Identity")]
    //[SerializeField] private string _weaponName;
    [Header("Visuals & Animation")]
    [SerializeField] private GameObject _weaponPrefab;
    [SerializeField] private AnimatorOverrideController _weaponOverride;
    [Header("Combat Stats")]
    [SerializeField] private float _damage;
    [SerializeField] private float _knockbackForce;
    [SerializeField] private float _staminaCost;
    [SerializeField] private int _level = 0;

    #region - Public Proprierties
    public GameObject WeaponPrefab => _weaponPrefab;
    public AnimatorOverrideController WeaponOverride => _weaponOverride;
    public float Damage
    {
        get => _damage;
        set => _damage = value;
    }
    public float KnockbackForce => _knockbackForce;
    public float StaminaCost => _staminaCost;   
    public int Level
    {
        get => _level;
        set => _level = value;
    }
    #endregion
}
