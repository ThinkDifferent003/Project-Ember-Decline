using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPotion", menuName = "Inventory/Items/Potion")]
public class PotionData : ItemData
{
    [SerializeField] private float _healAmount;
    public float HealAmount => _healAmount;

    public override void UseItem()
    {

    }
}
