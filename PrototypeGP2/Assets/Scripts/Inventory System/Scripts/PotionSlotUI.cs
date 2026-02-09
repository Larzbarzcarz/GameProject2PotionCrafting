using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PotionSlotUI : MonoBehaviour
{
    public PotionBaseSO potion;
    public PotionThrowController throwController;

    public Image iconImage;
    public TextMeshProUGUI amountText;

    public void Setup(PotionBaseSO potionData, int amount, PotionThrowController controller)
    {
        potion = potionData;
        throwController = controller;

        if (iconImage != null)
            iconImage.sprite = potionData.itemSprite;

        if (amountText != null)
            amountText.text = amount.ToString();
    }

    public void OnClick()
    {
        if (potion == null || throwController == null) return;
        throwController.SelectPotion(potion);
        Debug.Log($"PotionSlotUI: Selected {potion.name}");
    }
}
