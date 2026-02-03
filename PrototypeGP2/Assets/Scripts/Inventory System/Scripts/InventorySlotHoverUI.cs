using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public InventoryObject inventory;
    [HideInInspector] public InventorySlot boundSlot;
    public PotionInfoUI infoUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("HOVER ENTER!");
        if (inventory == null || boundSlot == null || boundSlot.item == null)
            return;

        if (!inventory.database.GetItemByStableId.TryGetValue(boundSlot.item.StableId, out var itemSO))
            return;

        infoUI.ShowInfoForSlot(itemSO, boundSlot.item.VariantKey, boundSlot.amount);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoUI.Hide();
    }
}
