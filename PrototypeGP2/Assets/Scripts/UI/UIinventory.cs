using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class UIinventory : MonoBehaviour
{
    public List<UIitem> UIitems = new List<UIitem>();
    public GameObject slotPrefab;
    public Transform slotPanel;
    public InventoryObject inventory;
    public List<Item> playerItems;
    public InventoryUI inventoryUI;
    public int numberOfSlots = 4;

    private void Awake()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject instance = Instantiate(slotPrefab);
            instance.transform.SetParent(slotPanel, false);
          UIitems.Add(instance.GetComponentInChildren<UIitem>());
        }
        
        
    }
	
    public void UpdateSlot(int slot, Item item)
    {
        if (slot < 0 || slot >= UIitems.Count) return;
        UIitems[slot].UpdateItem(item);
    }



    public void AddItem(ItemScriptableObject itemSO)
    {
        Item item = new Item(itemSO);
        playerItems.Add(item);
    }


    public void RemoveItem(Item item)
    {
        int index = UIitems.FindIndex(i => i.item == item);
        if (index != -1)
            UpdateSlot(index, null);
    }



}
