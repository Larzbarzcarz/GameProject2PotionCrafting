using UnityEngine;

public class CauldronUndoUI : MonoBehaviour
{
    [SerializeField] private CauldronContents cauldron;
    [SerializeField] private InventoryObject inventory;
    [SerializeField] private InventoryUI inventoryUI;

    // stack -> last in first out
    public void UndoLast()
    {
        if (!cauldron.TryPopLast(out var stableId))
            return;

        var so = inventory.database.GetItemByStableId[stableId];
        inventory.AddItem(new Item(so), 1, "");

        inventoryUI.Refresh();
    }
}
