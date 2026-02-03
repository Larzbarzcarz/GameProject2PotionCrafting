using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Info")]
    [SerializeField] private Image              icon;
    [SerializeField] private TextMeshProUGUI    amountText;

    [HideInInspector] public InventoryObject    inventory;
    [HideInInspector] public InventorySlot      boundSlot;

    private Canvas          rootCanvas;
    private RectTransform   dragIconRT;
    private Image           dragIconImg;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (boundSlot == null || boundSlot.amount <= 0)
            return;

        var go = new GameObject("DragIcon");
        go.transform.SetParent(rootCanvas.transform, false);
        dragIconRT = go.AddComponent<RectTransform>();
        dragIconImg = go.AddComponent<Image>();
        dragIconImg.raycastTarget = false;
        dragIconImg.sprite = icon.sprite;

        dragIconRT.sizeDelta = new Vector2(64, 64);
        dragIconRT.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconRT != null)
            dragIconRT.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIconRT != null)
            Destroy(dragIconRT.gameObject);
    }
}
