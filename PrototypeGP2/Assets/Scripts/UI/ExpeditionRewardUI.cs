using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExpeditionRewardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform rewardsParent;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private ItemDatabaseObject database;
    [SerializeField] private GameObject inventoryBackground; // To hide the "RawImage" scroll

    public void DisplayRewards(List<Item> rewards)
    {
        if (database == null)
        {
            Debug.LogError("[ExpeditionRewardUI] Database is missing!");
            return;
        }

        // Hide background at the VERY START
        if (inventoryBackground != null) inventoryBackground.SetActive(false);

        // Build database lookup if empty (safety)
        if (database.GetItemByStableId.Count == 0) database.BuildLookup();

        // specific cleanup of previous children if any
        foreach (Transform child in rewardsParent)
        {
            Destroy(child.gameObject);
        }

        // Group rewards
        Dictionary<string, (Item item, int count)> groupedRewards = new Dictionary<string, (Item, int)>();
        foreach (var r in rewards)
        {
            if (groupedRewards.ContainsKey(r.StableId))
            {
                var val = groupedRewards[r.StableId];
                val.count++;
                groupedRewards[r.StableId] = val;
            }
            else
            {
                groupedRewards[r.StableId] = (r, 1);
            }
        }

        Debug.Log($"[ExpeditionRewardUI] Instantiating {groupedRewards.Count} grouped items into {rewardsParent.name}.");

        foreach (var entry in groupedRewards.Values)
        {
            GameObject obj = Instantiate(inventoryItemPrefab, rewardsParent);

            // Layout Fixes
            obj.transform.localScale = Vector3.one;
            RectTransform rect = obj.GetComponent<RectTransform>();
            if (rect != null && rewardsParent.GetComponent<LayoutGroup>() == null)
            {
                // If there's no layout group, they all stack at 0,0. 
                // This is just a safety reset.
                rect.anchoredPosition = Vector3.zero;
            }

            // Force all children of the slot (Icon, Text) to be active
            foreach (Transform t in obj.GetComponentsInChildren<Transform>(true)) t.gameObject.SetActive(true);

            InventorySlotUI slotUI = obj.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                // LOOKUP the SO from the database
                if (database.GetItemByStableId.TryGetValue(entry.item.StableId, out var itemSO))
                {
                    slotUI.Set(itemSO, entry.count);
                    Debug.Log($"[ExpeditionRewardUI] Slot SET for: {itemSO.ItemName} x{entry.count}");
                }
                else
                {
                    Debug.LogWarning($"[ExpeditionRewardUI] Item {entry.item.StableId} not found in database!");
                }
            }
            else
            {
                Debug.LogWarning("[ExpeditionRewardUI] Prefab is missing 'InventorySlotUI' component!");
            }

            // Disable button interaction
            Button btn = obj.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
        }
    }
}
