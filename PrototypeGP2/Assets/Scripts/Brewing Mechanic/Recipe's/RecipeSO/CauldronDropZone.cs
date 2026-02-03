using UnityEngine;
using UnityEngine.EventSystems;

public class CauldronDropZone : MonoBehaviour, IDropHandler
{
    public CauldronContents cauldron;

    public void OnDrop(PointerEventData eventData)
    {
        var slotUI = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (slotUI == null || slotUI.boundSlot == null)
            return;

        cauldron.AddIngredients(slotUI.boundSlot.item.StableId);
    }
}
