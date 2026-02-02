using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PotionInfoUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PotionVariantRegistry  registry;
    [SerializeField] private TextMeshProUGUI        titleText;
    [SerializeField] private TextMeshProUGUI        bodyText;
    [SerializeField] private Image                  iconImage;

    private void Awake()
    {
        Hide();
    }

    public void ShowInfoForSlot(ItemScriptableObject itemSO, string variantKey, int amount)
    {
        if (itemSO == null)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);

        if (iconImage != null) iconImage.sprite = itemSO.itemSprite;
        if (iconImage != null) titleText.text = itemSO.ItemName != "" ? itemSO.ItemName : itemSO.name;

        string text = $"Amount: {amount}\n\n{itemSO.description}";

        if (itemSO.itemType == ItemType.Potion &&
            !string.IsNullOrEmpty(variantKey) &&
            registry != null &&
            registry.TryGet(variantKey, out var r))
        {
            if (iconImage != null && r.icon != null)
                iconImage.sprite = r.icon;

            text =
                $"Effect: {r.effect}\n" +
                $"Potency: {r.potency:0.0}\n" +
                $"Duration: {r.duration:0.0}s\n\n" +
                $"{itemSO.description}\n\n" +
                $"Key: {r.variantKey}\n" +
                $"Amount: {amount}";
        }

        if (bodyText != null) bodyText.text = text;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
