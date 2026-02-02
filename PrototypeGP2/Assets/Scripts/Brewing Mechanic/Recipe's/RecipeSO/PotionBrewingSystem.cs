using System;
using System.Linq;
using UnityEngine;

public enum PotionEffectType { Heal, Damage, Mystery }

public class PotionBrewingSystem : MonoBehaviour
{
    [SerializeField] private InventoryObject inventory;
    [SerializeField] private CauldronContents cauldron;
    [SerializeField] private PotionNameRegistry nameRegistry;
    [SerializeField] private ItemScriptableObject potionBaseSO;
    [SerializeField] private PotionVariantRegistry variantRegistry;
    [SerializeField] private PotionIconLibrary iconLibrary;

    [Header("Config")]
    [SerializeField] private int minIngredients = 2;

    public bool TryBrew(out string brewedVariantKey, out PotionEffectType effectType)
    {
        brewedVariantKey = "";
        effectType = default;

        if (cauldron.Sequence.Count < minIngredients)
            return false;


        //var needed = cauldron.Sequence
        //            .GroupBy(id => id)
        //            .ToDictionary(g => g.Key, g => g.Count());

        //foreach (var kv in needed)
        //    if (!inventory.HasItem(kv.Key, kv.Value))
        //        return false;

        //foreach (var kv in needed)
        //    inventory.RemoveItem(kv.Key, kv.Value);

        brewedVariantKey = string.Join("_", cauldron.Sequence);

        int seed = StableSeedFromString(brewedVariantKey);
        Debug.Log($"[BREW] Key={brewedVariantKey}");
        Debug.Log($"[BREW] Seed={seed}");

        //val av effekt
        int effectCount = Enum.GetValues(typeof(PotionEffectType)).Length;
        effectType = (PotionEffectType)(Math.Abs(seed) % effectCount);

        int n = cauldron.Sequence.Count; //stats baserat på seed och antal ingredienser
        float t01 = (Math.Abs(seed) % 1000) / 1000; //^

        float potencyBase = Mathf.Lerp(10f, 40f, t01);
        float durationBase = Mathf.Lerp(2f, 8f, 1f - t01); //tweakbara basvärden

        float potency = potencyBase + Mathf.Max(0, n - 2) * 6f;
        float duration = durationBase + Mathf.Max(0, n - 2) * 1.25f; //ju fler ingredienser desto starkare potion

        if (variantRegistry != null)
        {
            var result = variantRegistry.GetOrCreate(brewedVariantKey);
            result.seed = seed;
            result.effect = effectType;
            result.potency = potency;
            result.duration = duration;

            if (iconLibrary != null)
            {
                result.icon = iconLibrary.GetIcon(effectType);
            }
        }

        Debug.Log($"[BREW] Effect roll={(int)effectType} ({effectType})");

        inventory.AddItem(new Item(potionBaseSO, brewedVariantKey), 1, brewedVariantKey);

        //if (!nameRegistry.HasName(brewedVariantKey))
        //    nameRegistry.SetName(brewedVariantKey, DefaultNameFor(effectType)); //använda för att sätta default namn på potion

        cauldron.Clear();
        return true;
    }

    private int StableSeedFromString(string s)
    {
        unchecked
        {
            int hash = 23;
            for (int i = 0; i < s.Length; i++)
                hash = hash * 31 + s[i];
            return hash;
        }
    }

    private string DefaultNameFor(PotionEffectType type)
    {
        return type switch
        { 
            PotionEffectType.Heal => "Healing Potion",
            PotionEffectType.Damage => "Damage Potion",
            PotionEffectType.Mystery => "Mystery Potion",
            _ => "Failed Potion"
        };
    }
}
