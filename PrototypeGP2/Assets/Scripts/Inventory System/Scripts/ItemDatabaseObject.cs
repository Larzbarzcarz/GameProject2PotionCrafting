using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemScriptableObject[] Items;
    public Dictionary<ItemScriptableObject, int> GetID = new Dictionary<ItemScriptableObject, int>();
    public Dictionary<int, ItemScriptableObject> GetItem = new Dictionary<int, ItemScriptableObject>();

    public void OnAfterDeserialize()
    {
        GetID = new Dictionary<ItemScriptableObject, int>();
        GetItem = new Dictionary<int, ItemScriptableObject>();
        for (int i = 0; i < Items.Length; i++)
        {
            GetID.Add(Items[i], i);
            GetItem.Add(i, Items[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        
    }
}
