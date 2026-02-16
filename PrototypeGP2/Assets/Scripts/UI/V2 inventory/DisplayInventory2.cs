using UnityEngine;

public class DisplayInventory2 : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryUI InventoryUI;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        //InventoryUI.UIitems.ForEach(slot => slot.UpdateItem(null));

        foreach (var slot in inventory.Container.Items)
        {
            if (slot.amount <= 0)
                continue;

            if (!inventory.database.GetItemByStableId.TryGetValue(
                    slot.item.StableId, out var itemSO))
                continue;

            if (itemSO.itemType != ItemType.Potion)
                continue;

       
            Item item = new Item(itemSO, slot.item.VariantKey);

            //uiInventory.AddItem(item);
        }
    }


}
