using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItems : MonoBehaviour
{
    public ItemScriptableObject item;

    public string StableId => item != null ? item.StableId : "";

    public void Initialize(ItemScriptableObject itemSO)
    {
        item = itemSO;
    }
}

