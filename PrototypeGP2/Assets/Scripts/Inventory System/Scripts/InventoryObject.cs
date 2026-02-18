using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;
using System;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/New Inventory")]
public class InventoryObject : ScriptableObject
{
    public event Action OnChanged;

    public string savePath;
    public ItemDatabaseObject database;
    public Inventory Container;

    private void NotifyChanged() => OnChanged?.Invoke();

    public int GetAmount(string stableId)
    {
        for (int i = 0; i < Container.Items.Count; i++)
        {
            if (Container.Items[i].item.StableId == stableId)
                return Container.Items[i].amount;
        }
        return 0;
    }

    public bool HasItem(string stableId, int amount) => GetAmount(stableId) >= amount;

    public bool RemoveItem(string stableId, int amount)
    {
        for(int i = 0;i < Container.Items.Count; i++)
        {
            var slot = Container.Items[i];

            if (slot.item.StableId == stableId)
            {
                if (slot.amount < amount)
                    return false;

                slot.amount -= amount;

                if (slot.amount <= 0)
                    Container.Items.RemoveAt(i);

                NotifyChanged();
                return true;
            }
        }
        return false;
    }

    private void OnEnable()
    {
        if (Container == null)
            Container = new Inventory();
    }
    public void AddItem(Item _item, int _amount, string variantKey)
    {
        Debug.Log($"Adding item: {_item.StableId} x{_amount} variant={variantKey}");

        _item.VariantKey = variantKey ?? "";

        for (int i = 0; i < Container.Items.Count; i++)
        {
            var slot = Container.Items[i];

            if (slot.item.StableId == _item.StableId && slot.item.VariantKey == _item.VariantKey)
            {
                slot.AddAmount(_amount);
                Debug.Log("Stacked onto existing slot.");
                return;
            }
        }

        Debug.Log("Created new slot.");
        Container.Items.Add(new InventorySlot(_item, _amount));
        NotifyChanged();
    }
    public void DebugPrint()
    {
        Debug.Log("---- INVENTORY ----");
        foreach (var slot in Container.Items)
            Debug.Log($"{slot.item.StableId} x{slot.amount} variant={slot.item.VariantKey}");
    }


    [ContextMenu("Save")]
    public void Save()
    {
        var data = new InventorySaveData();

        foreach (var slot in Container.Items)
        {
            data.slots.Add(new InventorySaveData.SlotData
            {
                stableId = slot.item.StableId,
                amount = slot.amount,
                variantKey = slot.item.VariantKey
            });
        }

        var json = JsonUtility.ToJson(data, true);
        var fullPath = Path.Combine(Application.persistentDataPath, savePath);
        var directory = Path.GetDirectoryName(fullPath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(fullPath, json);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        var path = Path.Combine(Application.persistentDataPath, savePath);
        if (!File.Exists(path))
        {
            Debug.Log("No inventory save found. Creating new inventory.");
            Container = new Inventory();
            Save();
            return;
        }

        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<InventorySaveData>(json);

        Container = new Inventory();
        foreach (var slot in data.slots)
        {
            if (!database.GetItemByStableId.ContainsKey(slot.stableId))
            {
                Debug.LogWarning($"Saved item '{slot.stableId}' is no longer in database.");
                continue;
            }

            //�terskapa item med stableid
            var itemSO = database.GetItemByStableId[slot.stableId];
            var item = new Item(itemSO);
            item.VariantKey = slot.variantKey ?? "";
            Container.Items.Add(new InventorySlot(item, slot.amount));
        }
        Debug.Log($"Inventory loaded. Slots: {Container.Items.Count}");
        NotifyChanged();
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
        NotifyChanged();
    }
}
[System.Serializable]
public class Inventory
{
    public List<InventorySlot> Items = new List<InventorySlot>();
}

[System.Serializable]
public class InventorySlot
{
    public int amount;
    public Item item;
    public InventorySlot(Item _item, int _amount)
    {
        item   = _item;
        amount = _amount;
    }

    public void AddAmount(int value) => amount += value;
}

[System.Serializable]
public class InventorySaveData
{
    public List<SlotData> slots = new();

    [System.Serializable]
    public class SlotData
    {
        public string stableId;
        public int amount;
        public string variantKey;
    }
}
