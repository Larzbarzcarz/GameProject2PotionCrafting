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

    public bool TryBrew(out string brewedVariantKey, out PotionEffectType effectType)
    {
        Debug.Log("=== BREW ATTEMPT ===");

        brewedVariantKey = "";
        effectType = default;

        if (cauldron.Sequence.Count != 2)
        {
            Debug.Log($"[BREW] Failed: need exactly 2 ingredients, had {cauldron.Sequence.Count}");
            return false;
        }

        string firstId = cauldron.Sequence[0];
        string secondId = cauldron.Sequence[1];



        if (!inventory.HasItem(firstId, 1) || !inventory.HasItem(secondId, 1))
        {
            Debug.Log("[BREW] Failed: missing ingredients in inventory.");
            return false;
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
            Debug.Log($"[BREW] No explicit recipe for {mainKey} + {baseKey}. Applying default mapping.");
            ApplyDefaultRecipe(mainKey, baseKey, out effectType, out entry);
            found = true; // Use the default entry
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
        }


        inventory.RemoveItem(firstId, 1);
        inventory.RemoveItem(secondId, 1);

        inventory.AddItem(new Item(potionBaseSO, brewedVariantKey), 1, brewedVariantKey);


        //sets default name on potion to be main ingredient + Potion
        if (nameRegistry != null && !nameRegistry.HasName(brewedVariantKey))
            nameRegistry.SetName(brewedVariantKey, $"{mainKey} Potion");

        Debug.Log("=== BREW END ===");

        cauldron.Clear();
        return true;
    }

    private void ApplyDefaultRecipe(MainKeyword main, BaseKeyword baseKey, out PotionEffectType type, out PotionRecipeSO.Entry entry)
    {
        type = PotionEffectType.Fail;
        entry = new PotionRecipeSO.Entry();
        entry.mainKey = main;
        entry.baseKey = baseKey;

        // Simplified mapping based on user request
        if (main == MainKeyword.Blood)
        {
            if (baseKey == BaseKeyword.Fungus) { type = PotionEffectType.Heal; entry.instant = true; entry.percentOfMaxHP = 0.2f; entry.effectDescription = "Heals 20% max HP instantly"; }
            else if (baseKey == BaseKeyword.Mineral) { type = PotionEffectType.Heal; entry.turns = 4; entry.percentOfMaxHP = 0.5f; entry.effectDescription = "Heals 50% max HP over 4 turns"; }
            else if (baseKey == BaseKeyword.Animal) { type = PotionEffectType.Damage; entry.instant = true; entry.damage = 10f; entry.effectDescription = "Deals a large burst of damage"; }
            else { type = PotionEffectType.Heal; entry.instant = true; entry.percentOfMaxHP = 0.1f; entry.effectDescription = "Heals 10% max HP instantly"; }
        }
        else if (main == MainKeyword.Poision)
        {
            if (baseKey == BaseKeyword.Animal) { type = PotionEffectType.Damage; entry.turns = 3; entry.damage = 15f; entry.effectDescription = "Deals 15 damage over 3 turns"; }
            else if (baseKey == BaseKeyword.Fungus) { type = PotionEffectType.Heal; entry.instant = true; entry.percentOfMaxHP = 0.2f; entry.effectDescription = "Neutralizing draft"; }
            else { type = PotionEffectType.Damage; entry.turns = 3; entry.damage = 9f; entry.effectDescription = "Deals 9 damage over 3 turns"; }
        }
        else if (main == MainKeyword.Rodent)
        {
            if (baseKey == BaseKeyword.Cursed) { type = PotionEffectType.Utility; entry.turns = 2; entry.effectDescription = "Stuns the target for 2 turns"; }
            else if (baseKey == BaseKeyword.Mineral) { type = PotionEffectType.Damage; entry.turns = 3; entry.damage = 6f; entry.effectDescription = "Rodent infestation damage"; }
            else { type = PotionEffectType.Damage; entry.instant = true; entry.damage = 4f; entry.effectDescription = "Rodent bite"; }
        }
        else if (main == MainKeyword.Stone)
        {
            if (baseKey == BaseKeyword.Animal) { type = PotionEffectType.Utility; entry.turns = 1; entry.effectDescription = "Stuns the target for 1 turn"; }
            else { type = PotionEffectType.Damage; entry.instant = true; entry.damage = 5f; entry.effectDescription = "Deals small physical damage"; }
        }
        else if (main == MainKeyword.Wing)
        {
            if (baseKey == BaseKeyword.Fungus) { type = PotionEffectType.Special; entry.instant = true; entry.multiplier = 5; entry.effectDescription = "Recovers 5 stamina instantly"; }
            else { type = PotionEffectType.Special; entry.turns = 3; entry.multiplier = 2; entry.effectDescription = "Gradual stamina recovery"; }
        }
        else if (main == MainKeyword.Crystalline)
        {
            type = PotionEffectType.Heal; entry.instant = true; entry.percentOfMaxHP = 0.15f; entry.effectDescription = "Focusing potion";
        }
        else
        {
            // Generic fallback for Soothing/Rancid/Others
            type = PotionEffectType.Heal; entry.instant = true; entry.percentOfMaxHP = 0.1f; entry.effectDescription = "Basic soothing potion";
        }

        entry.effectType = type;
    }
}
