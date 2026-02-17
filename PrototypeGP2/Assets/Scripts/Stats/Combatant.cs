using System;
using System.Collections.Generic;
using UnityEngine;



public abstract class Combatant : MonoBehaviour // IPotionTarget removed — PotionEffectApplier replaces it
{
    [Header("Base Stats")]
    [SerializeField] protected float speed;
    [SerializeField] protected float strength;
    [SerializeField] protected float agility;

    [Header("Health")]
    [SerializeField] protected float maxHealth = 10f;
    [SerializeField] protected float currentHealth;

    [Header("Stamina")]
    [SerializeField] protected int maxStamina = 10;
    [SerializeField] protected int currentStamina;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public int MaxStamina => maxStamina;
    public int CurrentStamina => currentStamina;

    // ──────────── Status Effects ────────────
    private List<StatusEffect> activeStatusEffects = new();

    public bool isDead => currentHealth <= 0;

    public event Action<int, int> OnStaminaChanged;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }
    // ──────────── OLD ApplyPotion — commented out, replaced by PotionEffectApplier ────────────
    /*
        public void ApplyPotion(PotionBaseSO potion)
        {
            if (potion == null || potion.recipe == null) return;
            if (potion.recipe.entries == null || potion.recipe.entries.Count == 0) return;

            var e = potion.recipe.entries[0];

            switch (e.effectType)
            {
                case PotionEffectType.Damage:
                    TakeDamage(e.damage);
                    Debug.Log($"Enemy takes {e.damage} dmg from {potion.ItemName}");
                    break;

                case PotionEffectType.Heal:
                    float healAmount = maxHealth * e.percentOfMaxHP;
                    currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
                    Debug.Log($"Healed {healAmount} ({e.percentOfMaxHP:P0} maxHP) from {potion.ItemName}. HP now: {currentHealth}");
                    break;
            }
        }
    */

    // ──────────── Status Effect Methods ────────────

    public void AddStatusEffect(StatusEffect effect)
    {
        activeStatusEffects.Add(effect);
        Debug.Log($"[{name}] Status effect added: {effect}");
    }

    /// <summary>
    /// process all active status effects at the start of each turn
    /// applies HoT/DoT, then ticks durations and removes expired effects
    /// </summary>
    public void ProcessStatusEffects()
    {
        for (int i = activeStatusEffects.Count - 1; i >= 0; i--)
        {
            var fx = activeStatusEffects[i];

            switch (fx.type)
            {
                case StatusEffect.StatusEffectType.HealPerTurn:
                    Heal(fx.value);
                    Debug.Log($"[{name}] HoT healed {fx.value:F1}. HP: {currentHealth:F1}");
                    break;

                case StatusEffect.StatusEffectType.DamagePerTurn:
                    TakeDamage(fx.value);
                    Debug.Log($"[{name}] DoT dealt {fx.value}. HP: {currentHealth:F1}");
                    break;
            }

            if (!fx.Tick())
            {
                Debug.Log($"[{name}] Status effect expired: {fx.type}");
                activeStatusEffects.RemoveAt(i);
            }
        }
    }

    /// <summary>returns true if any stun type effect is active</summary>
    public bool IsStunned()
    {
        foreach (var fx in activeStatusEffects)
            if (fx.type == StatusEffect.StatusEffectType.Stun && !fx.IsExpired)
                return true;
        return false;
    }

    /// <summary>returns total defence modifier from all active defence mod effects</summary>
    public float GetDefenceModifier()
    {
        float total = 0;
        foreach (var fx in activeStatusEffects)
            if (fx.type == StatusEffect.StatusEffectType.DefenceMod && !fx.IsExpired)
                total += fx.value;
        return total;
    }

    /// <summary>returns the highest active damage reflect fraction (0-1)</summary>
    public float GetDamageReflect()
    {
        float best = 0;
        foreach (var fx in activeStatusEffects)
            if (fx.type == StatusEffect.StatusEffectType.DamageReflect && !fx.IsExpired)
                best = Mathf.Max(best, fx.value);
        return best;
    }

    /// <summary>returns total flat damage boost from all active damage boost effects</summary>
    public float GetDamageBoost()
    {
        float total = 0;
        foreach (var fx in activeStatusEffects)
            if (fx.type == StatusEffect.StatusEffectType.DamageBoost && !fx.IsExpired)
                total += fx.value;
        return total;
    }

    /// <summary>heals combatant, capped at max health</summary>
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    /// <summary>restores stamina, capped at max</summary>
    public void RestoreStamina(int amount)
    {
        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    /// <summary>sets health to a percentage of max health (used by SwapHp)</summary>
    public void SetHealthPercent(float percent)
    {
        currentHealth = Mathf.Clamp(maxHealth * percent, 0f, maxHealth);
    }

    /// <summary>clears all status effects (call between battles)</summary>
    public void ClearStatusEffects()
    {
        activeStatusEffects.Clear();
    }
    public virtual void TakeDamage(float damage)
    {
        Debug.Log($"{name} took {damage} damage. HP now: {currentHealth}");
        damage = Mathf.Max(0, damage);
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();

        if (currentHealth < 100)
            Dying();
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} died");
    }

    public bool TrySpendStamina(int cost)
    {
        if (currentStamina < cost)
            return false;

        currentStamina -= cost;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        return true;
    }

    public virtual void ResetHealth()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        ClearStatusEffects();
        Debug.Log($"{name} reset to full health/stamina. Status effects cleared.");
    }

    public virtual float DealDamage()
    {
        return strength;
    }

    public virtual void Victory()
    {
        Debug.Log("Victory");
    }

    public virtual void ResetAnimator()
    {
        // WIP
    }

    public virtual void Dying()
    {

    }

}
