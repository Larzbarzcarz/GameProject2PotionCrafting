using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExpeditionRewardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform rewardsParent;
    [SerializeField] private GameObject inventoryItemPrefab;

    public void DisplayRewards(List<Item> rewards)
    {
        // specific cleanup of previous children if any
        foreach (Transform child in rewardsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in rewards)
        {
            GameObject obj = Instantiate(inventoryItemPrefab, rewardsParent);
            
            // Assuming the prefab structure matches PotionDisplayInventory's expectation:
            // Child 0 is Image
            // Component TextMeshProUGUI is quantity
            
            // Set Icon
            Image iconImage = obj.transform.GetChild(0).GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = item.icon;
            }

            // Set Count (always 1 per entry in the list for now, or we can group them)
            // For now, let's just show "1" or hide the text if it's 1-per-slot
            TextMeshProUGUI amountText = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (amountText != null)
            {
                amountText.text = "1"; 
            }
            
            // Optional: Disable button interaction if we don't want them clickable
            Button btn = obj.GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = false;
            }
        }
    }
}
