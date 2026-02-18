using System;
using System.Linq;
using UnityEngine;

public class PotionBrewingSystem : MonoBehaviour
{
    [SerializeField] private InventoryObject inventory;
    [SerializeField] private CauldronContents cauldron;
    [SerializeField] private PotionNameRegistry nameRegistry;
    [SerializeField] private PotionVariantRegistry variantRegistry;
    [SerializeField] private PotionRecipeSO recipeMap;
    [SerializeField] private PotionBaseSO potionBaseSO;
    [SerializeField] private PotionIconLibrary iconLibrary;
    public InventoryObject Inventory => inventory;

    public bool TryBrew(InventoryObject inventory, out string brewedVariantKey, out PotionEffectType effectType)
    {
        Debug.Log("=== BREW ATTEMPT ===");

        brewedVariantKey = "";
        effectType = default;
      
        if (cauldron.Sequence.Count != 2)
        {
            Debug.Log($"[BREW] Failed: need exactly 2 ingredients, had {cauldron.Sequence.Count}");
            return false;
        }
     foreach (var id in cauldron.Sequence)
{
    Debug.Log($"Trying to lookup id: '{id}'");
}
foreach (var kvp in inventory.database.GetItemByStableId)
{
    Debug.Log($"Database contains: '{kvp.Key}'");
}
        string firstId = cauldron.Sequence[0];
     
        string secondId = cauldron.Sequence[1];

Debug.Log("Inventory count for first: " + inventory.GetAmount(firstId));
Debug.Log("Inventory count for second: " + inventory.GetAmount(secondId));
if (inventory.database.GetItemByStableId.Count == 0)
{
    inventory.database.BuildLookup();
}
        

        if (!inventory.database.GetItemByStableId.TryGetValue(firstId, out var firstSO) ||
            !inventory.database.GetItemByStableId.TryGetValue(secondId, out var secondSO))
        {
            Debug.LogError("[BREW] Failed: ingredient stableId not found in database.");
            return false;
        }

        if (firstSO is not IngredientObject mainIng || secondSO is not IngredientObject baseIng)
        {
            Debug.LogError("[BREW] Failed: items are not IngredientObject.");
            return false;
        }



        var mainKey = mainIng.mainKeyword;
        var baseKey = baseIng.baseKeyword;

        brewedVariantKey = $"{mainKey}_{baseKey}";


        bool found = recipeMap.TryGet(mainKey, baseKey, out var entry);
        if (!found)
        {
            Debug.Log($"[BREW] No recipe for {mainKey} + {baseKey}. Brewing failed potion.");
            effectType = PotionEffectType.Fail;
        }
        else
        {
            effectType = entry.effectType;
        }

        if (variantRegistry != null)
        {
            var v = variantRegistry.GetOrCreate(brewedVariantKey);
            v.effect = effectType;

            Sprite chosen = potionBaseSO.itemSprite;

            if (found && entry.icon != null)
                chosen = entry.icon;
            else if (iconLibrary != null)
                chosen = iconLibrary.GetIcon(effectType, chosen);



            v.icon = chosen;

            Debug.Log($"[BREW] Icon chosen = {(chosen != null ? chosen.name : "NULL")}");

            if (found)
            {
                string playerName = nameRegistry != null
                        ? nameRegistry.GetName(brewedVariantKey, $"{mainKey} Potion")
                        : $"{mainKey} Potion";

                Debug.Log($"[BREW] Brewed '{playerName}' ({entry.effectType})");
                Debug.Log($"[BREW] Main={mainKey}, Base={baseKey}");
                Debug.Log($"[BREW] Effect: {entry.effectDescription}");
                Debug.Log($"[BREW] Stats: instant={entry.instant}, " +
                                        $"pctMaxHP={entry.percentOfMaxHP:P0}, " +
                                        $"turns={entry.turns}, damage={entry.damage}, " +
                                        $"defence={entry.defence}, " +
                                        $"multiplier={entry.multiplier}");

                v.instant           = entry.instant;
                v.turns             = entry.turns;
                v.percentOfMaxHP    = entry.percentOfMaxHP;
                v.damage            = entry.damage;
                v.defence           = entry.defence;
                v.multiplier        = entry.multiplier;
            }
            else
            {
                Debug.Log($"[BREW] Brewed Failed Potion. Main={mainKey}, Base={baseKey}. [BREW] Failed potion has no effect.");

                v.instant = false;
                v.turns = 0;
                v.percentOfMaxHP = 0;
                v.damage = 0;
                v.defence = 0;
                v.multiplier = 0;
            }
        }


     

        inventory.AddItem(new Item(potionBaseSO, brewedVariantKey), 1, brewedVariantKey);


        //sets default name on potion to be main ingredient + Potion
        if (nameRegistry != null && !nameRegistry.HasName(brewedVariantKey))
            nameRegistry.SetName(brewedVariantKey, $"{mainKey} Potion");

        Debug.Log("=== BREW END ===");

        cauldron.Clear();
        return true;
    }
}
