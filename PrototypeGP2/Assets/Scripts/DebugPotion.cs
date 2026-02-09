using UnityEngine;

public class DebugPotion : MonoBehaviour
{
    [SerializeField] private InventoryObject inventory;
    [SerializeField] private PotionBaseSO testPotion;
    [SerializeField] private int amount = 3;

    private void Start()
    {
        if (inventory == null || testPotion == null)
        {
            Debug.LogError("[TEST] Missing inventory or testPotion reference.");
            return;
        }

        inventory.AddItem(new Item(testPotion), amount, "");
        Debug.Log($"[TEST] Added {amount}x {testPotion.ItemName} to inventory.");
    }
}
