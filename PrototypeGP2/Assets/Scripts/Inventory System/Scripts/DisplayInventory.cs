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
    [SerializeField] private CauldronContents cauldron;
    [SerializeField] private PotionVariantRegistry potionVariantRegistry;
    private Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();
     private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        Refresh();
    }

    
 
    public void Refresh()
    {
        ClearDisplay();
        CreateDisplay();
    }

    private void ClearDisplay()
    {
        foreach (var kv in itemsDisplayed)
            if (kv.Value != null)
                Destroy(kv.Value);

        itemsDisplayed.Clear();
    }

    public void CreateDisplay()
    {
        for (int i = 0; i < inventory.Container.Items.Count; i++)
        {
            InventorySlot slot = inventory.Container.Items[i];
            InventorySlot capturedSlot = slot;

            var obj = Instantiate(inventoryPrefab, transform);
            obj.GetComponent<RectTransform>().anchoredPosition = (Vector2)GetPosition(i);

           
            if (!inventory.database.GetItemByStableId.TryGetValue(
                    slot.item.StableId, out var itemSO))
            {
                Debug.LogError($"[UI] Missing stableId in DB: {slot.item.StableId} ({slot.item.Name})");
                Destroy(obj);
                continue;
            }

         
            Sprite icon = itemSO.itemSprite;
            if (itemSO.itemType == ItemType.Potion &&
                potionVariantRegistry != null &&
                potionVariantRegistry.TryGet(slot.item.VariantKey, out var variant) &&
                variant.icon != null)
            {
                icon = variant.icon;
            }

            obj.transform.GetChild(0).GetComponent<Image>().sprite = icon;
            obj.GetComponentInChildren<TextMeshProUGUI>().text =
                slot.amount.ToString("n0");

      
            Button button = obj.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnInventoryClick(capturedSlot));

            itemsDisplayed.Add(capturedSlot, obj);
        }
    }

    public Vector2 GetPosition(int i)
    {
        return new Vector3(
            X_Start + (X_SpaceBetweenItems * (i % numberOfColumns)),
            Y_Start + (-Y_SpaceBetweenItems * (i / numberOfColumns)),
            0f
        );
    }

 

    private void OnInventoryClick(InventorySlot slot)
    {
        if (slot.amount <= 0)
            return;

     
        if (cauldron != null)
        {
            cauldron.AddIngredients(slot.item.StableId);
        }

      
        //SpawnWorldItem(slot);

        slot.amount--;
        if (slot.amount <= 0)
            inventory.Container.Items.Remove(slot);

        Refresh();
    }

    private void SpawnWorldItem(InventorySlot slot)
    {
        if (!inventory.database.GetItemByStableId.TryGetValue(
                slot.item.StableId, out var itemSO))
            return;

        if (itemSO.worldPrefab == null)
            return;

        Vector3 spawnPos =
            mainCamera.transform.position +
            mainCamera.transform.forward * 1.5f;

      
        if (Input.touchCount > 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            spawnPos = ray.GetPoint(1.0f);
        }

        Instantiate(itemSO.worldPrefab, spawnPos, Quaternion.identity);
    }
}
