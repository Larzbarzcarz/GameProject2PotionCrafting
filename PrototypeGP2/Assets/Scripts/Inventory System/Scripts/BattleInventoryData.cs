using System;

/// <summary>
/// Static data that carries the player's selected potions from the lab to battle.
/// Survives scene loads (like ExpeditionData).
/// </summary>
public static class BattleInventoryData
{
    public const int MaxSlots = 5;

    [Serializable]
    public class BattleSlot
    {
        public bool   occupied;
        public string stableId;
        public string variantKey;

        public void Clear()
        {
            occupied   = false;
            stableId   = "";
            variantKey = "";
        }
    }

    public static BattleSlot[] Slots = new BattleSlot[MaxSlots];

    static BattleInventoryData()
    {
        for (int i = 0; i < MaxSlots; i++)
            Slots[i] = new BattleSlot();
    }

    public static int FilledCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < MaxSlots; i++)
                if (Slots[i].occupied) count++;
            return count;
        }
    }

    /// <summary>
    /// Place a potion in the next empty slot. Returns the slot index, or -1 if full.
    /// </summary>
    public static int AddOne(string stableId, string variantKey)
    {
        for (int i = 0; i < MaxSlots; i++)
        {
            if (!Slots[i].occupied)
            {
                Slots[i].occupied   = true;
                Slots[i].stableId   = stableId;
                Slots[i].variantKey = variantKey;
                return i;
            }
        }
        return -1;
    }

    public static void ClearSlot(int index)
    {
        if (index >= 0 && index < MaxSlots)
            Slots[index].Clear();
    }

    public static void ClearAll()
    {
        for (int i = 0; i < MaxSlots; i++)
            Slots[i].Clear();
    }
}
