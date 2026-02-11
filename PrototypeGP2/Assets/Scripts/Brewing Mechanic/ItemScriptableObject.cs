using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item", menuName = "ScriptableObjects/Item", order = 1)]
public abstract class ItemScriptableObject : ScriptableObject
{
    [SerializeField, HideInInspector] private string stableId;
    public string StableId => stableId;

    public int Id;
    public string ItemName;
    public Sprite itemSprite;
    public GameObject worldPrefab;
    public ItemType itemType;
    [TextArea] public string itemDescription;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(stableId))
        {
            stableId = System.Guid.NewGuid().ToString("N");
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
public enum ItemType
{
    Ingredient,
    Potion
}

[System.Serializable]
public class Item
{
    public string Name;
    public Sprite icon;
    public string StableId;
    public string VariantKey;
    public Item(ItemScriptableObject item, string variantKey = "")
    {
        Name = item.name;
        icon = item.itemSprite;
        StableId = item.StableId;
        VariantKey = variantKey ?? "";
    }
}
