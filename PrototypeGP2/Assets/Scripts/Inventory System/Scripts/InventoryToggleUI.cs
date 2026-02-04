using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToggleUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private DisplayInventory displayInventory;

    public void ToggleInventory()
    {
        bool isOpen = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
            displayInventory.Refresh();
    }
}
