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
            v.icon = iconLibrary.GetIcon(effectType, potionBaseSO.itemSprite);
        }


        inventory.RemoveItem(firstId, 1);
        inventory.RemoveItem(secondId, 1);

        inventory.AddItem(new Item(potionBaseSO, brewedVariantKey), 1, brewedVariantKey);


        //sets default name on potion to be main ingredient + Potion
        if (nameRegistry != null && !nameRegistry.HasName(brewedVariantKey))
        {
            nameRegistry.SetName(brewedVariantKey, $"{mainKey} Potion");
        }

        if (found)
        {
            string playerName = nameRegistry != null
                ? nameRegistry.GetName(brewedVariantKey, entry.effectType.ToString())
                : entry.effectType.ToString();

            Debug.Log($"[BREW] Brewed '{playerName}' ({entry.effectType})");
            Debug.Log($"[BREW] Main={mainKey}, Base={baseKey}");
            Debug.Log($"[BREW] Effect: {entry.effectDescription}");
        }
        else
        {
            Debug.Log($"[BREW] Brewed Failed Potion. Main={mainKey}, Base={baseKey}");
        }

        Debug.Log("=== BREW END ===");

        cauldron.Clear();
        return true;
    }
}
