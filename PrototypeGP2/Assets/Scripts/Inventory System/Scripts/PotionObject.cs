using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory System/Items/Potion")]
public class PotionObject : ItemScriptableObject
{
    public float healing;
    public int attackBonus;
    public int defenceBonus;

    public void Awake()
    {
        itemType = ItemType.Potion;
    }
}
