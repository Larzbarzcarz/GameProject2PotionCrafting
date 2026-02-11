
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon;
    public TextMeshProUGUI label;
    
    private ItemScriptableObject currentItem;
    private int currentAmount;
    public System.Action<ItemScriptableObject> OnItemClicked;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;

        OnItemClicked?.Invoke(currentItem);
    }
    public void Clear()
    {
        currentItem = null;
        currentAmount = 0;
        
        itemIcon.sprite = null;
        itemIcon.color = Color.clear;

        if (label != null)
            label.text = "";
    }

    public void Set(ItemScriptableObject itemSO, int amount)
    {
        
        currentItem = itemSO;
        currentAmount = amount; 
        
        itemIcon.sprite = itemSO.itemSprite;
        itemIcon.color = Color.white;

        if (label != null)
            label.text = amount.ToString();
    }
}

