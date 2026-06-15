using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGear", menuName ="Inventory/Items/Gear")]
public class GearData : ItemData
{
    [SerializeField] private List<StatModifier> _modifier = new List<StatModifier>();
    public List<StatModifier> Modifier => _modifier;

    public override void UseItem()
    {
        base.UseItem();
    }
}
