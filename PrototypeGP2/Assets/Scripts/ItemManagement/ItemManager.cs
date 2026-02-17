using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ingredient instance stored in the inventory
/// not a ScriptableObject — just name + keywords
/// </summary>
[System.Serializable]
public struct IngredientInstance
{
    public string name;
    public MainKeyword mainKeyword;
    public BaseKeyword baseKeyword;

    public IngredientInstance(string name, MainKeyword main, BaseKeyword baseKw)
    {
        this.name = name;
        this.mainKeyword = main;
        this.baseKeyword = baseKw;
    }

    public override string ToString() => $"{name} (main:{mainKeyword}, base:{baseKeyword})";
}

/// <summary>
/// singleton manager for the three item collections:
/// - inventory (ingredients)
/// - stash (crafted potions in lab)
/// - pocket (potions taken to battle, max 5)
///
/// persists across scenes via DontDestroyOnLoad
/// </summary>
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    /// <summary>Ingredients the player owns.</summary>
    public List<IngredientInstance> Inventory { get; private set; } = new();

    /// <summary>Crafted potions stored in the lab.</summary>
    public List<PotionData> Stash { get; private set; } = new();

    /// <summary>Potions taken into battle (max 5).</summary>
    public List<PotionData> Pocket { get; private set; } = new();

    public const int MaxPocketSize = 5;

    // ──────────── Static ingredient definitions (all 11 from the design table) ────────────
    public static readonly IngredientInstance[] AllIngredients = new IngredientInstance[]
    {
        new("Batwing",             MainKeyword.Wing,        BaseKeyword.Animal),
        new("Blood Clot",         MainKeyword.Blood,       BaseKeyword.Animal),
        new("Crooked Mushroom",   MainKeyword.Poison,      BaseKeyword.Fungus),
        new("Crystal",            MainKeyword.Crystalline,  BaseKeyword.Mineral),
        new("Dungeon Rat",        MainKeyword.Rodent,      BaseKeyword.Animal),
        new("Void Leech",         MainKeyword.Eldritch,    BaseKeyword.Cursed),
        new("Shivering Eyeball",  MainKeyword.Sensory,     BaseKeyword.Animal),
        new("Haunted Bark",       MainKeyword.Poison,      BaseKeyword.Cursed),
        new("Petrified Mushroom", MainKeyword.Stone,       BaseKeyword.Fungus),
        new("Slime Blob",         MainKeyword.Poison,      BaseKeyword.Animal),
        new("Tomb Ash",           MainKeyword.Stone,       BaseKeyword.Cursed),
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ──────────── Inventory Operations ────────────

    /// <summary>
    /// Adds N random ingredients to the inventory.
    /// </summary>
    public void AddRandomIngredients(int count)
    {
        Debug.Log($"[ItemManager] === ADDING {count} RANDOM INGREDIENTS ===");
        for (int i = 0; i < count; i++)
        {
            var ingredient = AllIngredients[Random.Range(0, AllIngredients.Length)];
            Inventory.Add(ingredient);
            Debug.Log($"  [{i + 1}] {ingredient}");
        }
        Debug.Log($"[ItemManager] Inventory now has {Inventory.Count} ingredients.");
    }

    /// <summary>
    /// crafts a potion from 2 random ingredients in the inventory
    /// first ingredient provides MainKeyword, second provides BaseKeyword
    /// both ingredients consumed
    /// </summary>
    public PotionData CraftRandomPotion()
    {
        Debug.Log("[ItemManager] === CRAFT RANDOM POTION ===");

        if (Inventory.Count < 2)
        {
            Debug.LogWarning($"[ItemManager] Not enough ingredients to craft! Have {Inventory.Count}, need 2.");
            return null;
        }

        // pick two different random indices
        int idx1 = Random.Range(0, Inventory.Count);
        int idx2 = Random.Range(0, Inventory.Count - 1);
        if (idx2 >= idx1) idx2++; // ensure different

        var first = Inventory[idx1];
        var second = Inventory[idx2];

        Debug.Log($"  First ingredient (main): {first}");
        Debug.Log($"  Second ingredient (base): {second}");

        // create potion: first's mainKeyword + second's baseKeyword
        var potion = new PotionData(first.mainKeyword, second.baseKeyword);

        // remove ingredients (remove higher index first to avoid shifting)
        if (idx1 > idx2)
        {
            Inventory.RemoveAt(idx1);
            Inventory.RemoveAt(idx2);
        }
        else
        {
            Inventory.RemoveAt(idx2);
            Inventory.RemoveAt(idx1);
        }

        Stash.Add(potion);

        Debug.Log($"  Result: {potion}");
        Debug.Log($"[ItemManager] Inventory: {Inventory.Count} ingredients. Stash: {Stash.Count} potions.");

        return potion;
    }

    /// <summary>
    /// fills pocket with up to 5 random potions from stash
    /// potions are moved (removed from stash)
    /// </summary>
    public void FillPocketFromStash()
    {
        Debug.Log("[ItemManager] === FILLING POCKET FROM STASH ===");

        Pocket.Clear();

        if (Stash.Count == 0)
        {
            Debug.LogWarning("[ItemManager] Stash is empty! No potions to take to battle.");
            return;
        }

        int toTake = Mathf.Min(MaxPocketSize, Stash.Count);
        // shuffle stash indices and take first N
        var indices = new List<int>();
        for (int i = 0; i < Stash.Count; i++) indices.Add(i);

        // shuffle
        for (int i = indices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        var takenIndices = new List<int>();
        for (int i = 0; i < toTake; i++)
        {
            int idx = indices[i];
            Pocket.Add(Stash[idx]);
            takenIndices.Add(idx);
            Debug.Log($"  [{i + 1}] {Stash[idx]}");
        }

        // remove taken potions from stash (sort descending to avoid index shifting)
        takenIndices.Sort();
        takenIndices.Reverse();
        foreach (int idx in takenIndices)
            Stash.RemoveAt(idx);

        Debug.Log($"[ItemManager] Pocket: {Pocket.Count} potions. Stash remaining: {Stash.Count}.");
    }

    /// <summary>
    /// uses random potion from pocket on given target
    /// potion is consumed (removed from pocket)
    /// </summary>
    public PotionData UseRandomPotionOnTarget(Combatant target, Combatant casterMonster)
    {
        if (Pocket.Count == 0)
        {
            Debug.LogWarning("[ItemManager] Pocket is empty! No potions to use.");
            return null;
        }

        int idx = Random.Range(0, Pocket.Count);
        var potion = Pocket[idx];
        Pocket.RemoveAt(idx);

        Debug.Log($"[ItemManager] === USING POTION ON {target.name} ===");
        Debug.Log($"  Potion: {potion}");

        PotionEffectApplier.Apply(potion, target, casterMonster);

        Debug.Log($"  Pocket remaining: {Pocket.Count} potions.");
        return potion;
    }

    /// <summary>
    /// logs state of all three collections
    /// </summary>
    public void LogState()
    {
        Debug.Log("=== ITEM MANAGER STATE ===");
        Debug.Log($"INVENTORY ({Inventory.Count} ingredients):");
        foreach (var ing in Inventory) Debug.Log($"  - {ing}");
        Debug.Log($"STASH ({Stash.Count} potions):");
        foreach (var pot in Stash) Debug.Log($"  - {pot}");
        Debug.Log($"POCKET ({Pocket.Count} potions):");
        foreach (var pot in Pocket) Debug.Log($"  - {pot}");
        Debug.Log("=== END STATE ===");
    }
}
