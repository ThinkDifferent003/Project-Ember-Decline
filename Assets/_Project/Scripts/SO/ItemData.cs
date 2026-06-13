using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Potion, Weapon , Mineral}
[CreateAssetMenu(fileName = "NewObject", menuName = "Inventory")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string _itemID;
    [SerializeField] private string _itemName;
    [TextArea(3, 10)]
    [SerializeField] private string _description;
    [SerializeField] private Sprite _itemSprite;
    [SerializeField] private ItemType _itemType;
    [SerializeField] private bool _isStackable;

    public string ItemID => _itemID;
    public string ItemName => _itemName;
    public string Description => _description;
    public Sprite ItemSprite => _itemSprite;
    public bool IsStackable => _isStackable;
}
