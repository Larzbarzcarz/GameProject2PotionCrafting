using UnityEngine;

public class ItemOnClick : MonoBehaviour
{
    public InventoryObject inventory;
    public void AddToBrew()
    {
        Debug.Log("now we are cooking");
        var item = GetComponent<PickupItems>();
        if (item)
        {
            inventory.AddItem(new Item(item.item), 1, "");
            Destroy(gameObject);

        }
    }
}
