/// <summary>
/// turn-based status effect active on a Combatant.
/// created by potion effects and processed each combat turn.
/// </summary>
[System.Serializable]
public class StatusEffect
{
    public StatusEffectType type;
    public float value;
    public int turnsRemaining;  // -1 means "rest of combat"

    public bool IsExpired => turnsRemaining == 0;

    public enum StatusEffectType
    {
        HealPerTurn,
        DamagePerTurn,
        DefenceMod,      // positive = buff, negative = debuff
        Stun,
        DamageReflect,   // value = fraction (0.5 = 50%)
        DamageBoost      // flat bonus to outgoing damage
    }

    public StatusEffect(StatusEffectType type, float value, int turns)
    {
        this.type = type;
        this.value = value;
        this.turnsRemaining = turns;
    }

    /// <summary>
    /// decrements turn counter, call at the end of the turn.
    /// returns true if the effect is still active after ticking
    /// </summary>
    public bool Tick()
    {
        if (turnsRemaining > 0)
            turnsRemaining--;
        // turnsRemaining == -1 means permanent (rest of combat), never expires
        return !IsExpired;
    }

    public override string ToString()
    {
        string duration = turnsRemaining == -1 ? "permanent" : $"{turnsRemaining} turns left";
        return $"{type} (value={value}, {duration})";
    }
}
