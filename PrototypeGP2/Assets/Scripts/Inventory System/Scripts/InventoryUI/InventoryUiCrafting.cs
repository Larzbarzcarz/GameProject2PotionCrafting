using UnityEngine;
using System.Collections.Generic;

public class  InventoryUiCrafting  : MonoBehaviour
{
    public InventoryObject inventory;
    public List<InventorySlotUI> slots;

    [Header("Crafting")]
    [SerializeField] private Transform cauldronSpawnPoint;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, -0.08f, 0f);

    private void OnEnable()
    {
        Refresh();
    }

    private void Start()
    {
        foreach (var slot in slots)
        {
            slot.OnItemClicked += HandleItemClicked;
        }
    }

    private void HandleItemClicked(ItemScriptableObject item)
    {
        if (item == null) return;

        SpawnWorldItem(item);
        RemoveOneItem(item);
        Refresh();
    }

    private void SpawnWorldItem(ItemScriptableObject itemSO)
    {
        if (itemSO.worldPrefab == null)
        {
            Debug.LogWarning("Missing worldPrefab on: " + itemSO.name);
            return;
        }

        if (cauldronSpawnPoint == null)
        {
            Debug.LogWarning("Cauldron spawn point missing!");
            return;
        }

        Instantiate(
            itemSO.worldPrefab,
            cauldronSpawnPoint.position + spawnOffset,
            cauldronSpawnPoint.rotation
        );
    }

    private void RemoveOneItem(ItemScriptableObject itemSO)
    {
        for (int i = 0; i < inventory.Container.Items.Count; i++)
        {
            var slot = inventory.Container.Items[i];

            if (!inventory.database.GetItemByStableId.TryGetValue(
                slot.item.StableId, out var dbItem))
                continue;

            if (dbItem == itemSO)
            {
                slot.amount--;

                if (slot.amount <= 0)
                {
                    inventory.Container.Items.RemoveAt(i);
                }

                break;
            }
        }
    }

    public void Refresh()
    {
        foreach (var slotUI in slots)
            slotUI.Clear();

        for (int i = 0; i < inventory.Container.Items.Count && i < slots.Count; i++)
        {
            var slot = inventory.Container.Items[i];

            if (slot.amount <= 0)
                continue;

            if (!inventory.database.GetItemByStableId.TryGetValue(
                slot.item.StableId, out var itemSO))
                continue;

            slots[i].Set(itemSO, slot.amount);
        }
    }
}


