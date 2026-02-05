using System.Collections.Generic;
using UnityEngine;

/// 
public static class ExpeditionData
{
    public static List<Item> SelectedPotions = new List<Item>();
    public static int CurrentEncounter = 1;
    public static int TotalEncounters = 3;
    public static List<Item> LootedIngredients = new List<Item>();
    
    public static bool IsExpeditionActive = false;
    
    public static void StartExpedition(List<Item> potions)
    {
        SelectedPotions = new List<Item>(potions);
        CurrentEncounter = 1;
        LootedIngredients.Clear();
        IsExpeditionActive = true;
        Debug.Log($"[ExpeditionData] Started expedition with {SelectedPotions.Count} potions.");
    }
    
    public static bool AdvanceEncounter()
    {
        CurrentEncounter++;
        Debug.Log($"[ExpeditionData] Advanced to encounter {CurrentEncounter}/{TotalEncounters}");
        return CurrentEncounter <= TotalEncounters;
    }
    
    public static void CompleteExpedition()
    {
        Debug.Log($"[ExpeditionData] Expedition complete! Looted {LootedIngredients.Count} ingredients.");
        IsExpeditionActive = false;
    }
    
    public static void FailExpedition()
    {
        Debug.Log("[ExpeditionData] Expedition failed! Lost all potions and loot.");
        SelectedPotions.Clear();
        LootedIngredients.Clear();
        IsExpeditionActive = false;
    }
    
    public static bool UsePotion(int index)
    {
        if (index < 0 || index >= SelectedPotions.Count)
            return false;
        
        Item potion = SelectedPotions[index];
        SelectedPotions.RemoveAt(index);
        Debug.Log($"[ExpeditionData] Used potion: {potion.Name}. Remaining: {SelectedPotions.Count}");
        return true;
    }
    
    public static void Reset()
    {
        SelectedPotions.Clear();
        LootedIngredients.Clear();
        CurrentEncounter = 1;
        IsExpeditionActive = false;
    }
}
