using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Data")]
    public InventoryObject inventory;

    [Header("UI")]
    public Transform contentParent;
    public GameObject slotPrefab;
    public PotionThrowController throwController;

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (inventory == null || contentParent == null || slotPrefab == null)
        {
            Debug.LogError("InventoryUI missing references.");
            return;
        }

        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        foreach (var slot in inventory.Container.Items)
        {
            var go = Instantiate(slotPrefab, contentParent);

            var slotUI = go.GetComponent<PotionSlotUI>();
            if (slotUI == null)
            {
                Debug.LogError("Slot prefab is missing PotionSlotUI component.");
                continue;
            }

            if (!inventory.database.GetItemByStableId.ContainsKey(slot.item.StableId))
                continue;


            var itemSO = inventory.database.GetItemByStableId[slot.item.StableId];

            var potionSO = itemSO as PotionBaseSO;
            if (potionSO == null)
                continue;

            slotUI.Setup(potionSO, slot.amount, throwController);
        }
        Debug.Log($"InventoryUI refreshed. Slots: {contentParent.childCount}");
    }
}
