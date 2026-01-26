using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemScriptableObject[] Items;
    public Dictionary<int, ItemScriptableObject> GetItem = new Dictionary<int, ItemScriptableObject>();

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].Id = i;
            GetItem.Add(i, Items[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        GetItem = new Dictionary<int, ItemScriptableObject>();
    }
}
