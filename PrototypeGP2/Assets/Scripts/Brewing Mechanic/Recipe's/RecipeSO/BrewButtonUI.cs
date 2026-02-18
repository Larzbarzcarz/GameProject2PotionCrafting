using UnityEngine;

public class BrewButtonUI : MonoBehaviour
{
    [SerializeField] private PotionBrewingSystem brewer;
    [SerializeField] private InventoryUiCrafting  inventoryUI;
    [SerializeField] private PotionRenameUI renameUI;

    public void Brew()
    {
        if (brewer == null || inventoryUI == null)
        {
            Debug.LogError("[UI] Missing brewer or inventoryUI reference.");
            return;
        }

        bool success = brewer.TryBrew(inventoryUI.inventory, out var variantKey, out var effect);

        if (!success)
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
