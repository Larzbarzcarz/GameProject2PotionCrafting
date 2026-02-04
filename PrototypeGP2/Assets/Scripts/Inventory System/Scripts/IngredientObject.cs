using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainKeyword { Blood, Poision, Rodent, Crystalline, Eldritch, Sensory, Stone, Wing, Soap }
public enum BaseKeyword { Animal, Mineral, Fungus, Cursed, Unique }

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Crafting/Ingredient")]
public class IngredientObject : ItemScriptableObject
{
    public MainKeyword mainKeyword;
    public BaseKeyword baseKeyword;

    public void Awake()
    {
        itemType = ItemType.Ingredient;
    }
}
