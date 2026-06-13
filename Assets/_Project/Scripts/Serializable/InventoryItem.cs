using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    private ItemData _data;
    private int _count;
    public ItemData Data => _data;
    public int Count
    {
        get => _count;
        set => _count = value;
    }

    public InventoryItem(ItemData sourceData , int startingCount)
    {
        _data = sourceData;
        _count = startingCount;
    }
}
