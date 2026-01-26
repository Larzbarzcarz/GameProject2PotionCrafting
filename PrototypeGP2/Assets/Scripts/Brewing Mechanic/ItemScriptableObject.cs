using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item", menuName = "ScriptableObjects/Item", order = 1)]
public abstract class ItemScriptableObject : ScriptableObject
{
    public int Id;
    public string ItemName;
    public Sprite itemSprite;
    public ItemType itemType;
    [TextArea(15, 20)]
    public string description;
}
public enum ItemType
{
    Equipment,
    Ingredient,
    Default,
    Potion
}

[System.Serializable]
public class Item
{
    public string Name;
    public int Id;
    public Item(ItemScriptableObject item)
    {
        Name = item.name;
        Id = item.Id;
    }
}
