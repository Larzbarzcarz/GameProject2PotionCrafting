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
    public Camera mainCamera;

    private Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();
     private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        RebuildInventoryUI();
    }

    #region UI

    public void RebuildInventoryUI()
    {
       
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

     
        itemsDisplayed.Clear();

     
        for (int i = 0; i < inventory.Container.Items.Count; i++)
        {
            int index = i;
            InventorySlot slot = inventory.Container.Items[i];

            var obj = Instantiate(inventoryPrefab, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            if (slot.item != null &&
                inventory.database.GetItem.TryGetValue(slot.item.Id, out var itemData))
            {
                obj.transform.GetChild(0).GetComponent<Image>().sprite =
                    itemData.itemSprite;
            }

            obj.GetComponentInChildren<TextMeshProUGUI>().text =
                slot.amount.ToString("n0");

            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() => OnClickSpawn(index));

            itemsDisplayed.Add(slot, obj);
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(
            X_Start + (X_SpaceBetweenItems * (i % numberOfColumns)),
            Y_Start + (-Y_SpaceBetweenItems * (i / numberOfColumns)),
            0f
        );
    }

    #endregion

    #region Spawn Logic

    public void OnClickSpawn(int index)
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not assigned!");
            return;
        }

        if (index < 0 || index >= inventory.Container.Items.Count)
            return;

        InventorySlot slot = inventory.Container.Items[index];

        if (slot.item == null)
            return;

        if (!inventory.database.GetItem.TryGetValue(slot.item.Id, out var itemData))
        {
            Debug.LogError($"Item ID {slot.item.Id} not found in database!");
            return;
        }

       
        Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * 1.5f;


        if (Input.touchCount > 0)
        {
            Vector2 touchPos = Input.GetTouch(0).position;
            Ray ray = mainCamera.ScreenPointToRay(touchPos);
            spawnPosition = ray.GetPoint(1.0f); 
        }

     
        Instantiate(itemData.worldPrefab, spawnPosition, Quaternion.identity);

    
        slot.amount--;

        if (slot.amount <= 0)
        {
            inventory.Container.Items.RemoveAt(index);
        }

     
        RebuildInventoryUI();
    }

    #endregion
}


