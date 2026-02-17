using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainKeyword { Blood, Poison, Rodent, Crystalline, Eldritch, Sensory, Stone, Wing }
public enum BaseKeyword { Animal, Mineral, Fungus, Cursed }

public enum Rarity { Common, Rare, Legendary }

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Crafting/Ingredient")]
public class IngredientObject : ItemScriptableObject
{
    public MainKeyword mainKeyword;
    public BaseKeyword baseKeyword;
    public Rarity rarity;

    public void Awake()
    {
        itemType = ItemType.Ingredient;
    }
}
