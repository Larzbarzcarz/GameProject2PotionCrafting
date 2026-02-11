using UnityEngine;
using System.Collections.Generic;

using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public InventoryObject inventory;
    public List<InventorySlotUI> slots;
    public BattleSystem battleSystem;

    private void OnEnable()
    {
        Refresh();
    }
    private void HandleItemClicked(ItemScriptableObject item)
    {
        battleSystem.SpawnInventoryItem(item);
    }
    private void Start()
    {
        foreach (var slot in slots)
        {
            slot.OnItemClicked += HandleItemClicked;
        }
    }
    public void Refresh()
    {
    
        foreach (var slotUI in slots)
            slotUI.Clear();

       
        for (int i = 0; i < inventory.Container.Items.Count && i < slots.Count; i++)
        {
            InventorySlot slot = inventory.Container.Items[i];

            if (slot.amount <= 0)
                continue;

            if (!inventory.database.GetItemByStableId.TryGetValue(
                    slot.item.StableId, out var itemSO))
                continue;

            slots[i].Set(itemSO, slot.amount);
        }
    }
}





