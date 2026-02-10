
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI label;

    public void Clear()
    {
        itemIcon.sprite = null;
        itemIcon.color = Color.clear;

        if (label != null)
            label.text = "";
    }

    public void Set(ItemScriptableObject itemSO, int amount)
    {
        itemIcon.sprite = itemSO.itemSprite;
        itemIcon.color = Color.white;

        if (label != null)
            label.text = amount.ToString();
    }
}

