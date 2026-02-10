using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// In-battle panel that displays the potions the player brought.
/// Tapping a potion uses it (debug log for now).
/// </summary>
public class BattlePotionPanel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ItemDatabaseObject    database;
    [SerializeField] private PotionVariantRegistry variantRegistry;

    [Header("UI")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform  contentParent;

    private List<GameObject> spawnedSlots = new List<GameObject>();

    // ────────────────────────────────────────────────────────────────
    // Public API
    // ────────────────────────────────────────────────────────────────

    public void Toggle()
    {
        if (gameObject.activeSelf)
            Close();
        else
            Open();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    // ────────────────────────────────────────────────────────────────
    // Display
    // ────────────────────────────────────────────────────────────────

    private void Refresh()
    {
        foreach (var go in spawnedSlots)
            if (go != null) Destroy(go);
        spawnedSlots.Clear();

        for (int i = 0; i < BattleInventoryData.MaxSlots; i++)
        {
            var slot = BattleInventoryData.Slots[i];
            if (!slot.occupied) continue;

            if (!database.GetItemByStableId.TryGetValue(slot.stableId, out var itemSO))
                continue;

            var obj = Instantiate(slotPrefab, contentParent);

            // icon
            Sprite icon = itemSO.itemSprite;
            if (variantRegistry != null &&
                variantRegistry.TryGet(slot.variantKey, out var variant) &&
                variant.icon != null)
            {
                icon = variant.icon;
            }

            var img = obj.transform.GetChild(0).GetComponent<Image>();
            if (img != null) img.sprite = icon;

            // amount text — always "1" since each slot is one potion
            var txt = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = "1";

            // click to use
            int capturedIndex = i;
            var btn = obj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnPotionUsed(capturedIndex));
            }

            spawnedSlots.Add(obj);
        }
    }

    // ────────────────────────────────────────────────────────────────
    // Use potion
    // ────────────────────────────────────────────────────────────────

    private void OnPotionUsed(int slotIndex)
    {
        var slot = BattleInventoryData.Slots[slotIndex];
        if (!slot.occupied) return;

        // Resolve a display name
        string potionName = slot.variantKey;
        if (database.GetItemByStableId.TryGetValue(slot.stableId, out var itemSO))
            potionName = itemSO.ItemName;

        Debug.Log($"You just used {potionName}!");

        // TODO: apply actual potion effects here in a future update

        BattleInventoryData.ClearSlot(slotIndex);
        Refresh();
    }
}
