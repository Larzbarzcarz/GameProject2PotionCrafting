using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Inventory System/Items/Ingredient")]
public class IngredientObject : ItemScriptableObject
{
    public void Awake()
    {
        itemType = ItemType.Ingredient;
    }
}
