using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon" , menuName = "Weapons")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private string _weaponName;
    [SerializeField] private GameObject _weaponPrefab;
    [SerializeField] private AnimatorOverrideController _weaponOverride;
    [SerializeField] private float _damage;
    [SerializeField] private float _knockbackForce;

    public GameObject WeaponPrefab => _weaponPrefab;
    public AnimatorOverrideController WeaponOverride => _weaponOverride;
    public float Damage => _damage;
    public float KnockbackForce => _knockbackForce;
}
