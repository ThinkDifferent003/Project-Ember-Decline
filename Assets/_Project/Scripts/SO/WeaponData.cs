using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon" , menuName = "Weapons")]
public class WeaponData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string _weaponName;
    [Header("Visuals & Animation")]
    [SerializeField] private GameObject _weaponPrefab;
    [SerializeField] private AnimatorOverrideController _weaponOverride;
    [Header("Combat Stats")]
    [SerializeField] private float _damage;
    [SerializeField] private float _knockbackForce;

    #region - Public Proprierties
    public GameObject WeaponPrefab => _weaponPrefab;
    public AnimatorOverrideController WeaponOverride => _weaponOverride;
    public float Damage => _damage;
    public float KnockbackForce => _knockbackForce;
    #endregion
}
