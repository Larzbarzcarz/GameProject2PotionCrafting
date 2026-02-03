using UnityEngine;

public class BrewButtonUI : MonoBehaviour
{
    [SerializeField] private PotionBrewingSystem brewer;
    [SerializeField] private DisplayInventory inventoryUI;

    public void Brew()
    {
        if (brewer.TryBrew(out var variantKey, out var effect))
        {
            Debug.Log($"Potion brewed: {effect}");
            inventoryUI.Refresh();
        }
        else
        {
            Debug.Log("Brew failed - not enough ingredients!");
        }
    }
}
