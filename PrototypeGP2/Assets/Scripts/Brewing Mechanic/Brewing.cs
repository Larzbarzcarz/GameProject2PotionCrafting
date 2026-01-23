using System;
using UnityEngine;
using System.Collections.Generic;

public class Brewing : MonoBehaviour
{
    
    public InventoryObject inventory;
     public List<CraftingRecipe> craftingRecipes;
    
     public void Craft(CraftingRecipe recipe)
     {
         if (CanCraft(recipe))
         {
             Debug.Log("cooking");
             ConsumeIngredients(recipe);
              CreateResult(recipe);
         }
         else
         {
             Debug.Log("Cannot be crafted");
         }
         
     }

     private bool CanCraft(CraftingRecipe recipe)
     {
         foreach (var ingredient in recipe.ingredients)
         {
             int itemCount = 0;
             foreach (var item in inventory.Container )
             {
                 if (Equals(item, ingredient.item)) 
                     {
                     itemCount++;
                     }

                 if (itemCount < ingredient.amount)
                 {
                     return false;
                 }
             }
         }
         return true;
     }

     private void ConsumeIngredients(CraftingRecipe recipe)
     {
         foreach (var ingredient in recipe.ingredients)
             for (int i = 0; i < ingredient.amount; i++)
             {
                 //inventory.RemoveItem(ingredient.item);
             }
     }

     private void CreateResult(CraftingRecipe recipe)
     {
         for (int i = 0; i < recipe.resultAmount; i++)
         {
             //inventory.AddItem(recipe.result);
         }
     }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Material")
        {
            Debug.Log("Now we are cooking");
            // Craft(CraftingRecipe())
           
        }
        
        
    }

   
  





}
