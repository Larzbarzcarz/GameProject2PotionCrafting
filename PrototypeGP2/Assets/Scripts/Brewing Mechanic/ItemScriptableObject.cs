using UnityEngine;


[CreateAssetMenu(fileName = "item", menuName = "ScriptableObjects/Item", order = 1)]
public abstract class ItemScriptableObject : ScriptableObject
{

    public string ItemName;
    public Sprite itemSprite;
    public ItemType itemType;
    public GameObject prefab;
    [TextArea(15, 20)]
    public string description;
}
public enum ItemType
{
    Equipment,
    Mushroom,
    Default,
    Potion
}


