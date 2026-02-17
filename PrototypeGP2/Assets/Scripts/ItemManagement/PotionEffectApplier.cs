using UnityEngine;

/// <summary>
/// applies a PotionData's effect to a target Combatant
/// instant effects happen immediately; timed effects add StatusEffects to the target
/// </summary>
public static class PotionEffectApplier
{
    /// <summary>
    /// apply a potion's effect to a target combatant
    /// casterMonster is needed for SwapHp and for mutation tracking
    /// </summary>
    public static void Apply(PotionData potion, Combatant target, Combatant casterMonster)
    {
        if (potion == null)
        {
            Debug.LogWarning("[PotionEffectApplier] Null potion, nothing to apply.");
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("[PotionEffectApplier] Null target, nothing to apply.");
            return;
        }

        var fx = potion.effect;
        Debug.Log($"[PotionEffectApplier] Applying {potion} on {target.name}");

        switch (fx.effectId)
        {
            case PotionEffectId.HealInstant:
                float healAmt = target.MaxHealth * fx.value;
                target.Heal(healAmt);
                Debug.Log($"  -> Healed {healAmt:F1} HP ({fx.value:P0} of max). HP now: {target.CurrentHealth:F1}");
                break;

            case PotionEffectId.HealOverTime:
                float hotPerTurn = target.MaxHealth * fx.value;
                target.AddStatusEffect(new StatusEffect(
                    StatusEffect.StatusEffectType.HealPerTurn, hotPerTurn, fx.turns));
                Debug.Log($"  -> HoT applied: {hotPerTurn:F1} HP/turn for {fx.turns} turns");
                break;

            case PotionEffectId.DamageInstant:
                target.TakeDamage(fx.value);
                Debug.Log($"  -> Dealt {fx.value} damage. HP now: {target.CurrentHealth:F1}");
                break;

            case PotionEffectId.DamageOverTime:
                target.AddStatusEffect(new StatusEffect(
                    StatusEffect.StatusEffectType.DamagePerTurn, fx.value, fx.turns));
                string dotDur = fx.turns == -1 ? "rest of combat" : $"{fx.turns} turns";
                Debug.Log($"  -> DoT applied: {fx.value} dmg/turn for {dotDur}");
                break;

            case PotionEffectId.DefenceUp:
                target.AddStatusEffect(new StatusEffect(
                    StatusEffect.StatusEffectType.DefenceMod, fx.value, fx.turns));
                string defUpDur = fx.turns == -1 ? "rest of combat" : $"{fx.turns} turns";
                Debug.Log($"  -> Defence +{fx.value} for {defUpDur}");
                break;

            case PotionEffectId.DefenceDown:
                target.AddStatusEffect(new StatusEffect(
                    StatusEffect.StatusEffectType.DefenceMod, -fx.value, fx.turns));
                string defDnDur = fx.turns == -1 ? "rest of combat" : $"{fx.turns} turns";
                Debug.Log($"  -> Defence -{fx.value} for {defDnDur}");
                break;

            case PotionEffectId.Stun:
                target.AddStatusEffect(new StatusEffect(
                    StatusEffect.StatusEffectType.Stun, 0f, fx.turns));
                Debug.Log($"  -> Stunned for {fx.turns} turns");
                break;

            case PotionEffectId.StaminaRestore:
                int staminaAmt = Mathf.RoundToInt(target.MaxStamina * fx.value);
                target.RestoreStamina(staminaAmt);
                Debug.Log($"  -> Restored {staminaAmt} stamina ({fx.value:P0} of max). Stamina now: {target.CurrentStamina}");
                break;

            case PotionEffectId.DamageReflect:
                target.AddStatusEffect(new StatusEffect(
                    StatusEffect.StatusEffectType.DamageReflect, fx.value, fx.turns));
                Debug.Log($"  -> Damage reflect {fx.value:P0} for {fx.turns} turns");
                break;

            case PotionEffectId.SwapHp:
                if (casterMonster == null || target == casterMonster)
                {
                    Debug.Log("  -> SwapHp: no valid swap target, doing nothing.");
                    break;
                }
                float monsterPct = casterMonster.CurrentHealth / casterMonster.MaxHealth;
                float targetPct = target.CurrentHealth / target.MaxHealth;
                casterMonster.SetHealthPercent(targetPct);
                target.SetHealthPercent(monsterPct);
                Debug.Log($"  -> Swapped HP%. Monster: {monsterPct:P0}->{targetPct:P0}, Target: {targetPct:P0}->{monsterPct:P0}");
                break;

            case PotionEffectId.DamageBoost:
                target.AddStatusEffect(new StatusEffect(
                    StatusEffect.StatusEffectType.DamageBoost, fx.value, fx.turns));
                Debug.Log($"  -> Damage boost +{fx.value} for {fx.turns} turns");
                break;

            case PotionEffectId.None:
            default:
                Debug.Log("  -> No effect.");
                break;
        }

        // connect with mutation system
        if (MutationRegistry.Instance != null)
        {
            MutationRegistry.Instance.OnPotionConsumed(potion.baseKeyword);
            Debug.Log($"  -> Mutation tracked: {potion.baseKeyword}");
        }
    }
}
