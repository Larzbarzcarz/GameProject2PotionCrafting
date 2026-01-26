using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class DisplayInventory : MonoBehaviour
{
    public InventoryObject inventory;
    public GameObject inventoryPrefab;

    public int X_Start;
    public int Y_Start;
    public int X_SpaceBetweenItems;
    public int Y_SpaceBetweenItems;
    public int numberOfColumns;
    
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();

    void Start()
    {
        CreateDisplay();
    }

    void Update()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        
        for (int i = 0; i < inventory.Container.Items.Count; i++)
        {
            InventorySlot slot = inventory.Container.Items[i];

            if (itemsDisplayed.ContainsKey(slot))
            {
                itemsDisplayed[slot].GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
            }
            else
            {
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
                if (!inventory.database.GetItem.ContainsKey(slot.item.Id))
                {
                    Debug.LogError(
                        $"Item ID {slot.item.Id} not found in ItemDatabase! " +
                        $"Check your ItemDatabaseObject asset."
                    );
                    continue;
                }
                obj.transform.GetChild(0).GetComponentInChildren<UnityEngine.UI.Image>().sprite = inventory.database.GetItem[slot.item.Id].itemSprite;
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
                
                itemsDisplayed.Add(slot, obj);
                
                //itemsDisplayed.Add(inventory.Container.Items[i], obj);
                var button = obj.GetComponent<Button>();
                Debug.Log("Button found: " + button);
                button.onClick.AddListener(() => OnclickSpawn(slot));
            }
            
        }
    }

    public void OnclickSpawn(InventorySlot slot)
    {     Debug.Log("Inventory item clicked!");
        if (slot.amount <= 0)
            return;
        
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 2f;
        var itemData = inventory.database.GetItem[slot.item.Id];
        if (itemData.worldPrefab == null)
        {
            Debug.LogError($"Item '{itemData.name}' has NO worldPrefab assigned!");
            return;
        }
        Instantiate(itemData.worldPrefab, spawnPosition, Quaternion.identity);
        slot.amount -= 1;

        if (slot.amount <= 0)
        {
            inventory.Container.Items.Remove(slot);
            Destroy(itemsDisplayed[slot]);
            itemsDisplayed.Remove(slot);

        }
        
    }

    public void CreateDisplay()
    {
        for (int i = 0; i < inventory.Container.Items.Count; i++)
        {
            InventorySlot slot = inventory.Container.Items[i];

            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            InventorySlot capturedSlot = slot; 
            obj.GetComponent<Button>().onClick.AddListener(() => OnclickSpawn(capturedSlot));
            obj.transform.GetChild(0).GetComponentInChildren<UnityEngine.UI.Image>().sprite = inventory.database.GetItem[slot.item.Id].itemSprite;
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
            itemsDisplayed.Add(slot, obj);
            var button = obj.GetComponent<Button>();
            Debug.Log("Button found: " + button);
            button.onClick.AddListener(() => OnclickSpawn(slot));
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(X_Start + (X_SpaceBetweenItems * (i % numberOfColumns)), Y_Start + (-Y_SpaceBetweenItems * (i / numberOfColumns)), 0f);
    }
}


