using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public Button BackButton; // assign in prefab inspector

    void Start()
    {
        // Hook Back button click
        if (BackButton != null)
            BackButton.onClick.AddListener(OnBackClicked);

        Debug.Log("Inventory UI initialized. Back button ready.");
    }

    // ---------- BACK BUTTON LOG ----------
    public void OnBackClicked()
    {
        Debug.Log("Back button clicked in Inventory. (Test UI Event: Inventory Close)");
        // Programmer note: Hook real inventory close logic here
    }

    // ---------- PUBLIC METHODS FOR TESTING ----------
    public void OpenInventory()
    {
        Debug.Log("Inventory Open event triggered. (Test UI Event)");
    }

    public void CloseInventory()
    {
        Debug.Log("Inventory Close event triggered. (Test UI Event)");
    }
}