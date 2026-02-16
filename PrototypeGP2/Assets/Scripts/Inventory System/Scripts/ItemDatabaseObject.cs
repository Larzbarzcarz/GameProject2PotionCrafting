using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Database")]
public class ItemDatabaseObject : ScriptableObject
{
    public ItemScriptableObject[] Items;
    [System.NonSerialized]
    public Dictionary<string, ItemScriptableObject> GetItemByStableId = new Dictionary<string, ItemScriptableObject>();

    public void OnAfterDeserialize()
    {
		GetItemByStableId.Clear();

        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == null)
                continue;

            Items[i].Id = i;

            var key = Items[i].StableId;
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError($"Item '{Items[i].name}' missing StableId. Open asset so that OnValidate runs.");
                continue;
            }

            if (GetItemByStableId.ContainsKey(key))
                Debug.LogError($"DUPLICATE StableId! '{Items[i].name}' another item has the same StableId.");

            GetItemByStableId[key] = Items[i];
        }
    }

    private void OnEnable()
    {
        BuildLookup();
    }

    public void BuildLookup()
    {
        GetItemByStableId.Clear();

        if (Items == null)
            return;

        for (int i = 0;i < Items.Length; i++)
        {
            var it = Items[i];
            if (it == null)
                continue;

            it.Id = i;

            var key = it.StableId;
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError($"ItemDatabaseObject: Item at index {i} has empty StableId.");
                continue;
            }

            if (GetItemByStableId.ContainsKey(key))
            {
                Debug.LogError($"ItemDatabaseObject: Duplicate StableId at index {i}. Key={key}");
                continue;
            }

            GetItemByStableId[key] = it;
        }
    }
}
