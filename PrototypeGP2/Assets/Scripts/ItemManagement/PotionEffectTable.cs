using System.Collections.Generic;

/// <summary>
/// static lookup table mapping every (MainKeyword, BaseKeyword) pair to a PotionEffectInfo
/// </summary>
public static class PotionEffectTable
{
  private static readonly Dictionary<(MainKeyword, BaseKeyword), PotionEffectInfo> table;

  static PotionEffectTable()
  {
    table = new Dictionary<(MainKeyword, BaseKeyword), PotionEffectInfo>
        {
            // ──────────── BLOOD ────────────
            { (MainKeyword.Blood, BaseKeyword.Mineral),
              new PotionEffectInfo(PotionEffectId.HealOverTime, 0.125f, 4,
                  "Heals 50% max HP over 4 turns (12.5% per turn)") },

            { (MainKeyword.Blood, BaseKeyword.Fungus),
              new PotionEffectInfo(PotionEffectId.HealInstant, 0.20f, 0,
                  "Heals 20% max HP instantly") },

            { (MainKeyword.Blood, BaseKeyword.Cursed),
              new PotionEffectInfo(PotionEffectId.DamageReflect, 1.0f, 2,
                  "Reflects 100% of damage taken for 2 turns") },

            { (MainKeyword.Blood, BaseKeyword.Animal),
              new PotionEffectInfo(PotionEffectId.DamageInstant, 8f, 0,
                  "Deals a large burst of 8 damage") },

            // ──────────── CRYSTALLINE ────────────
            { (MainKeyword.Crystalline, BaseKeyword.Mineral),
              new PotionEffectInfo(PotionEffectId.DefenceUp, 3f, -1,
                  "Boosts Defence by 3 for the rest of combat") },

            { (MainKeyword.Crystalline, BaseKeyword.Fungus),
              new PotionEffectInfo(PotionEffectId.DefenceUp, 6f, 1,
                  "Massively boosts Defence by 6 for 1 turn") },

            { (MainKeyword.Crystalline, BaseKeyword.Cursed),
              new PotionEffectInfo(PotionEffectId.DefenceDown, 99f, 1,
                  "Reduces Defence to 0 for 1 turn") },

            { (MainKeyword.Crystalline, BaseKeyword.Animal),
              new PotionEffectInfo(PotionEffectId.DamageReflect, 0.5f, 2,
                  "Reflects 50% of damage taken for 2 turns") },

            // ──────────── ELDRITCH ────────────
            { (MainKeyword.Eldritch, BaseKeyword.Mineral),
              new PotionEffectInfo(PotionEffectId.HealInstant, 0.10f, 0,
                  "Heals 10% max HP instantly") },

            { (MainKeyword.Eldritch, BaseKeyword.Fungus),
              new PotionEffectInfo(PotionEffectId.SwapHp, 0f, 0,
                  "Swaps the HP percentage between your monster and the target") },

            { (MainKeyword.Eldritch, BaseKeyword.Cursed),
              new PotionEffectInfo(PotionEffectId.DamageInstant, 5f, 0,
                  "Deals 5 damage") },

            { (MainKeyword.Eldritch, BaseKeyword.Animal),
              new PotionEffectInfo(PotionEffectId.Stun, 0f, 1,
                  "Stuns the target for 1 turn") },

            // ──────────── SENSORY ────────────
            { (MainKeyword.Sensory, BaseKeyword.Mineral),
              new PotionEffectInfo(PotionEffectId.DamageBoost, 3f, 3,
                  "Boosts outgoing damage by 3 for 3 turns") },

            { (MainKeyword.Sensory, BaseKeyword.Fungus),
              new PotionEffectInfo(PotionEffectId.HealInstant, 0.10f, 0,
                  "Heals 10% max HP instantly") },

            { (MainKeyword.Sensory, BaseKeyword.Cursed),
              new PotionEffectInfo(PotionEffectId.DamageInstant, 3f, 0,
                  "Deals 3 damage") },

            { (MainKeyword.Sensory, BaseKeyword.Animal),
              new PotionEffectInfo(PotionEffectId.Stun, 0f, 1,
                  "Stuns the target for 1 turn") },

            // ──────────── POISON ────────────
            { (MainKeyword.Poison, BaseKeyword.Mineral),
              new PotionEffectInfo(PotionEffectId.DamageOverTime, 1f, -1,
                  "Applies weak poison: 1 damage per turn for rest of combat") },

            { (MainKeyword.Poison, BaseKeyword.Fungus),
              new PotionEffectInfo(PotionEffectId.HealInstant, 0.10f, 0,
                  "Heals 10% max HP instantly") },

            { (MainKeyword.Poison, BaseKeyword.Cursed),
              new PotionEffectInfo(PotionEffectId.DefenceUp, 2f, 3,
                  "Boosts Defence by 2 for 3 turns") },

            { (MainKeyword.Poison, BaseKeyword.Animal),
              new PotionEffectInfo(PotionEffectId.DamageInstant, 5f, 0,
                  "Applies a medium burst of 5 poison damage") },

            // ──────────── RODENT ────────────
            { (MainKeyword.Rodent, BaseKeyword.Mineral),
              new PotionEffectInfo(PotionEffectId.DamageOverTime, 2f, 3,
                  "Deals 2 damage each turn for 3 turns (rats biting)") },

            { (MainKeyword.Rodent, BaseKeyword.Fungus),
              new PotionEffectInfo(PotionEffectId.DamageBoost, 3f, 1,
                  "Boosts outgoing damage by 3 for 1 turn") },

            { (MainKeyword.Rodent, BaseKeyword.Cursed),
              new PotionEffectInfo(PotionEffectId.Stun, 0f, 2,
                  "Stuns the target for 2 turns") },

            { (MainKeyword.Rodent, BaseKeyword.Animal),
              new PotionEffectInfo(PotionEffectId.Stun, 0f, 2,
                  "Stuns the target for 2 turns") },

            // ──────────── STONE ────────────
            { (MainKeyword.Stone, BaseKeyword.Mineral),
              new PotionEffectInfo(PotionEffectId.DamageInstant, 3f, 0,
                  "Deals 3 physical damage (throwing a rock)") },

            { (MainKeyword.Stone, BaseKeyword.Fungus),
              new PotionEffectInfo(PotionEffectId.DefenceUp, 2f, 3,
                  "Boosts Defence by 2 for 3 turns") },

            { (MainKeyword.Stone, BaseKeyword.Cursed),
              new PotionEffectInfo(PotionEffectId.DamageInstant, 5f, 0,
                  "Deals 5 damage") },

            { (MainKeyword.Stone, BaseKeyword.Animal),
              new PotionEffectInfo(PotionEffectId.Stun, 0f, 1,
                  "Stuns the target for 1 turn") },

            // ──────────── WING ────────────
            { (MainKeyword.Wing, BaseKeyword.Mineral),
              new PotionEffectInfo(PotionEffectId.StaminaRestore, 0.25f, 0,
                  "Recovers 25% stamina instantly") },

            { (MainKeyword.Wing, BaseKeyword.Fungus),
              new PotionEffectInfo(PotionEffectId.StaminaRestore, 0.50f, 0,
                  "Recovers 50% stamina instantly") },

            { (MainKeyword.Wing, BaseKeyword.Cursed),
              new PotionEffectInfo(PotionEffectId.DamageInstant, 3f, 0,
                  "Deals 3 damage") },

            { (MainKeyword.Wing, BaseKeyword.Animal),
              new PotionEffectInfo(PotionEffectId.DamageBoost, 3f, 3,
                  "Boosts outgoing damage by 3 for 3 turns") },
        };
  }

  /// <summary>
  /// looks up effect for a given keyword combination.
  /// returns "no effect" fallback if the combination isn't found (we'd like this to never happen)
  /// </summary>
  public static PotionEffectInfo Lookup(MainKeyword main, BaseKeyword baseKw)
  {
    if (table.TryGetValue((main, baseKw), out var info))
      return info;

    UnityEngine.Debug.LogWarning($"[PotionEffectTable] No entry for {main}+{baseKw}, returning None.");
    return new PotionEffectInfo(PotionEffectId.None, 0, 0, "No effect");
  }
}
