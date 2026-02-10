using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Lab-scene panel: lets the player pick up to 5 individual potions from
/// their stash to bring into battle.
///
/// Stash area shows potion stacks (icon + amount).
/// Battle-slot area shows 5 fixed slots.
/// Tap stash → move one to next empty battle slot.
/// Tap battle slot → return that potion to stash.
/// "Go!" always active; loads battle scene with current selection.
/// </summary>
public class PotionSelectionPanel : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log($"[PotionSelectionPanel] Awake called. Active: {gameObject.activeSelf}", gameObject);
        // Force-hide on Awake to guarantee it starts hidden
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Debug.Log($"[PotionSelectionPanel] OnEnable called. Stack trace:\n{System.Environment.StackTrace}", gameObject);
    }
    [Header("Data")]
    [SerializeField] private InventoryObject   labInventory;
    [SerializeField] private ItemDatabaseObject database;
    [SerializeField] private PotionVariantRegistry variantRegistry;

    [Header("UI — Stash")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform  stashContent;

    [Header("UI — Battle Slots")]
    [SerializeField] private Transform[] battleSlotParents = new Transform[5];

    [Header("UI — Controls")]
    [SerializeField] private Button goButton;

    [Header("Scene")]
    [SerializeField] private int battleSceneIndex = 1;

    // ── runtime state ──────────────────────────────────────────────
    // Each entry mirrors one potion stack visible in the stash grid.
    private class StashEntry
    {
        public string stableId;
        public string variantKey;
        public int    displayAmount;   // amount currently shown (original minus picked)
        public int    originalAmount;
        public GameObject uiObject;
    }

    private List<StashEntry> stashEntries = new List<StashEntry>();

    // What is in each battle slot (null = empty).
    private class BattleSlotState
    {
        public string     stableId;
        public string     variantKey;
        public GameObject uiObject;
    }

    private BattleSlotState[] battleSlots = new BattleSlotState[BattleInventoryData.MaxSlots];

    // ────────────────────────────────────────────────────────────────
    // Public API
    // ────────────────────────────────────────────────────────────────

    /// <summary>Call from Clickforcamera to open the panel.</summary>
    public void Open()
    {
        BattleInventoryData.ClearAll();

        for (int i = 0; i < battleSlots.Length; i++)
            battleSlots[i] = null;

        gameObject.SetActive(true);
        RebuildStash();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    // ────────────────────────────────────────────────────────────────
    // Stash display
    // ────────────────────────────────────────────────────────────────

    private void RebuildStash()
    {
        // clear old UI
        foreach (var entry in stashEntries)
        {
            if (entry.uiObject != null)
                Destroy(entry.uiObject);
        }
        stashEntries.Clear();

        // clear battle slot UI
        for (int i = 0; i < battleSlots.Length; i++)
        {
            if (battleSlots[i] != null && battleSlots[i].uiObject != null)
                Destroy(battleSlots[i].uiObject);
            battleSlots[i] = null;
        }

        if (labInventory == null) return;

        foreach (var slot in labInventory.Container.Items)
        {
            if (slot.amount <= 0) continue;
            if (!database.GetItemByStableId.TryGetValue(slot.item.StableId, out var itemSO))
                continue;
            if (itemSO.itemType != ItemType.Potion) continue;

            var entry = new StashEntry
            {
                stableId       = slot.item.StableId,
                variantKey     = slot.item.VariantKey,
                displayAmount  = slot.amount,
                originalAmount = slot.amount
            };

            entry.uiObject = CreateSlotUI(stashContent, itemSO, slot.item.VariantKey, entry.displayAmount);
            int capturedIndex = stashEntries.Count;
            entry.uiObject.GetComponent<Button>().onClick.AddListener(() => OnStashTapped(capturedIndex));
            stashEntries.Add(entry);
        }
    }

    // ────────────────────────────────────────────────────────────────
    // Interactions
    // ────────────────────────────────────────────────────────────────

    private void OnStashTapped(int stashIndex)
    {
        if (stashIndex < 0 || stashIndex >= stashEntries.Count) return;

        var entry = stashEntries[stashIndex];
        if (entry.displayAmount <= 0) return;
        if (BattleInventoryData.FilledCount >= BattleInventoryData.MaxSlots) return;

        // find next empty battle slot
        int slotIdx = -1;
        for (int i = 0; i < battleSlots.Length; i++)
        {
            if (battleSlots[i] == null)
            {
                slotIdx = i;
                break;
            }
        }
        if (slotIdx < 0) return;

        // move one potion
        entry.displayAmount--;
        RefreshStashSlotUI(stashIndex);

        // fill battle slot
        if (!database.GetItemByStableId.TryGetValue(entry.stableId, out var itemSO))
            return;

        var bsState = new BattleSlotState
        {
            stableId   = entry.stableId,
            variantKey = entry.variantKey,
            uiObject   = CreateSlotUI(battleSlotParents[slotIdx], itemSO, entry.variantKey, 1)
        };

        int capturedSlot = slotIdx;
        bsState.uiObject.GetComponent<Button>().onClick.AddListener(() => OnBattleSlotTapped(capturedSlot));
        battleSlots[slotIdx] = bsState;
    }

    private void OnBattleSlotTapped(int slotIdx)
    {
        if (slotIdx < 0 || slotIdx >= battleSlots.Length) return;
        var bs = battleSlots[slotIdx];
        if (bs == null) return;

        // return potion to stash
        foreach (var entry in stashEntries)
        {
            if (entry.stableId == bs.stableId && entry.variantKey == bs.variantKey)
            {
                entry.displayAmount++;
                RefreshStashSlotUI(stashEntries.IndexOf(entry));
                break;
            }
        }

        // clear battle slot
        if (bs.uiObject != null)
            Destroy(bs.uiObject);
        battleSlots[slotIdx] = null;
    }

    // ────────────────────────────────────────────────────────────────
    // "Go!" button
    // ────────────────────────────────────────────────────────────────

    /// <summary>Wire this to the Go button's onClick in the Inspector.</summary>
    public void OnGoClicked()
    {
        BattleInventoryData.ClearAll();

        for (int i = 0; i < battleSlots.Length; i++)
        {
            if (battleSlots[i] != null)
            {
                BattleInventoryData.Slots[i].occupied   = true;
                BattleInventoryData.Slots[i].stableId   = battleSlots[i].stableId;
                BattleInventoryData.Slots[i].variantKey  = battleSlots[i].variantKey;
            }
        }

        Debug.Log($"[PotionSelectionPanel] Going to battle with {BattleInventoryData.FilledCount} potions.");
        SceneManager.LoadSceneAsync(battleSceneIndex);
    }

    // ────────────────────────────────────────────────────────────────
    // Helpers
    // ────────────────────────────────────────────────────────────────

    private GameObject CreateSlotUI(Transform parent, ItemScriptableObject itemSO,
                                     string variantKey, int amount)
    {
        var obj = Instantiate(slotPrefab, parent);

        // icon
        Sprite icon = itemSO.itemSprite;
        if (variantRegistry != null &&
            variantRegistry.TryGet(variantKey, out var variant) &&
            variant.icon != null)
        {
            icon = variant.icon;
        }

        var img = obj.transform.GetChild(0).GetComponent<Image>();
        if (img != null) img.sprite = icon;

        // amount text
        var txt = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null) txt.text = amount.ToString("n0");

        return obj;
    }

    private void RefreshStashSlotUI(int stashIndex)
    {
        if (stashIndex < 0 || stashIndex >= stashEntries.Count) return;
        var entry = stashEntries[stashIndex];
        if (entry.uiObject == null) return;

        // update amount text
        var txt = entry.uiObject.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null) txt.text = entry.displayAmount.ToString("n0");

        // hide if 0
        entry.uiObject.SetActive(entry.displayAmount > 0);
    }
}
