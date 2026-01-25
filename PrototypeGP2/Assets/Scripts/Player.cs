using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;

    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<Items>();
        if (item)
        {
            inventory.AddItem(item.item, 1);
            Destroy(other.gameObject);
            inventory.Save();
            Debug.Log("INVENTORY SAVED AUTOMATICALLY");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
            Debug.Log("SAVED");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            inventory.Load();
            Debug.Log("LOADED");
        }
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }
}
