using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]

public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    public void AddItem(Item _item, int _amount)
    {
        bool hasItem = false;
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == _item)
            {
                Container[i].AddAmount(_amount);
                hasItem = true;
                break;
            }
        }
        if (!hasItem)
        {
            Container.Add(new InventorySlot(_item, _amount));
        }
       
    }

    public void RemoveItem(Item _item, int _amount)
    {
        bool hasItem = true;
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == _item)
            {
                Container[i].RemoveAmount(_amount);
                hasItem = false;
                break;

            }

            if (!hasItem)
            {
                Container.Remove(new InventorySlot(_item, _amount));
            }
            
        }
    }

    public void RemoveItem(Item ingredientItem)
    {
        RemoveItem(ingredientItem, 1);
    }

    public void AddItem(Item recipeResult)
    {
        AddItem(recipeResult, 1);
    }
}

[System.Serializable]
public class InventorySlot
{
    public int amount;
    public Item item;
    public InventorySlot(Item _item, int _amount)
    {
        this.item   = _item;
        this.amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }
    public void RemoveAmount(int value)
    {
        amount -= value;
    }
  
}
