using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Crafting/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName = "New Recipe";
    public List<ItemAmount> ingredients;
    public ItemScriptableObject result;
    public int resultAmount = 1;
}

[System.Serializable]
public struct ItemAmount
{
    public ItemScriptableObject item;
    public int amount;
}