using System;
using UnityEngine;
using System.Collections.Generic;

public class Brewing : MonoBehaviour
{
    public InventoryObject inventory;
    public List<CraftingRecipe> craftingRecipes;

    public void Craft(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe, out string missingItems))
        {
            Debug.Log($"Cannot be crafted: {missingItems}");
            return;
        }

        //ConsumeIngredients(recipe);
        CreateResult(recipe);

        Debug.Log($"Crafted: {recipe.recipeName}");
    }

    private bool CanCraft(CraftingRecipe recipe, out string missingItems)
    {
        missingItems = "";

        foreach (var ingredient in recipe.ingredients)
        {
            int have = inventory.GetAmount(ingredient.item.StableId);
            if (have < ingredient.amount)
            {
                int missing = ingredient.amount - have;
                missingItems += $"{ingredient.item.name} missing {missing}.";
            }
        }

        return string.IsNullOrEmpty(missingItems);
    }

    private void ConsumeIngredients(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            inventory.RemoveItem(ingredient.item.StableId, ingredient.amount);
        }
    }

    private void CreateResult(CraftingRecipe recipe)
    {
        var item = new Item(recipe.result);
        inventory.AddItem(item, recipe.resultAmount, "");
    }


    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("now we are cooking");
        var item = other.GetComponent<PickupItems>();
        if (item)
        {
            inventory.AddItem(new Item(item.item), 1, "");
            Destroy(other.gameObject);

        }
    }

    
}
