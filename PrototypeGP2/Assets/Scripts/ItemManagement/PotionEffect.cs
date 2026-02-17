using System;

/// <summary>
/// all implementable potion effect types
/// </summary>
public enum PotionEffectId
{
    None,
    HealInstant,      // heals X% of max HP instantly
    HealOverTime,     // heals X% of max HP per turn for N turns
    DamageInstant,    // deals X damage instantly
    DamageOverTime,   // deals X damage per turn for N turns (poison)
    DefenceUp,        // adds defence modifier for N turns
    DefenceDown,      // reduces defence for N turns
    Stun,             // target can't act for N turns
    StaminaRestore,   // restores X% stamina instantly
    DamageReflect,    // reflects X% of incoming damage for N turns
    SwapHp,           // swaps HP percentages between caster's monster and target
    DamageBoost       // increases outgoing damage for N turns
}

/// <summary>
/// describes parameters of a specific potion effect
/// </summary>
[Serializable]
public struct PotionEffectInfo
{
    public PotionEffectId effectId;
    public float value;          // damage amount, heal %, defence mod, reflect %, boost amount
    public int turns;            // 0 = instant, -1 = rest of combat, N = N turns
    public string description;   // human-readable effect description

    public PotionEffectInfo(PotionEffectId id, float value, int turns, string description)
    {
        this.effectId = id;
        this.value = value;
        this.turns = turns;
        this.description = description;
    }

    public bool IsInstant => turns == 0;
}
