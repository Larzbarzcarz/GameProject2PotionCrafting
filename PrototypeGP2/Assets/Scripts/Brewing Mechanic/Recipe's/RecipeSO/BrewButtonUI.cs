using UnityEngine;

public class BrewButtonUI : MonoBehaviour
{
    [SerializeField] private PotionBrewingSystem brewer;
    [SerializeField] private InventoryUiCrafting  inventoryUI;
    [SerializeField] private PotionRenameUI renameUI;

    public void Brew()
    {
        brewer.TryBrew(inventoryUI.inventory, out var variantKey, out var effect);
        {
            Debug.Log("Brew failed - not enough ingredients!");
            return;
        }
    
        Debug.Log($"Potion brewed: {effect} (key={variantKey})");
        inventoryUI.Refresh();


        if (renameUI != null)
            renameUI.StartRename(variantKey);
    }
}
