using System;
using System.Collections.Generic;
using UnityEngine;



public abstract class Combatant : MonoBehaviour
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

    public bool isDead => currentHealth <= 0;

    public event Action<int, int> OnStaminaChanged;

    [Header("Status Effects")]
    protected List<StatusEffectInstance> activeEffects = new List<StatusEffectInstance>();
    public bool isStunned { get; private set; }

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
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

    public virtual void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"{name} healed by {amount}. HP now: {currentHealth}");
    }

    public virtual void HealStamina(int amount)
    {
        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        Debug.Log($"{name} recovered {amount} stamina. Stamina now: {currentStamina}");
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
        return true;
    }

    public virtual void ResetHealth()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        activeEffects.Clear();
        isStunned = false;
        Debug.Log($"{name} reset to full health/stamina and effects cleared.");
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

    #region Status Effects

    public void ApplyStatusEffect(StatusEffectType type, int turns, float value)
    {
        activeEffects.Add(new StatusEffectInstance(type, turns, value));
        Debug.Log($"Applied {type} to {name} for {turns} turns.");
    }

    public void ProcessTurnEffects()
    {
        isStunned = false;
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];
            
            switch (effect.type)
            {
                case StatusEffectType.Regen:
                    Heal(maxHealth * effect.value);
                    break;
                case StatusEffectType.Poison:
                    TakeDamage(effect.value);
                    break;
                case StatusEffectType.Stun:
                    isStunned = true;
                    break;
                case StatusEffectType.StaminaRegen:
                    HealStamina((int)effect.value);
                    break;
            }

            effect.turnsRemaining--;
            if (effect.turnsRemaining <= 0)
            {
                activeEffects.RemoveAt(i);
            }
        }
    }

    #endregion
}

public enum StatusEffectType
{
    Regen,
    Poison,
    Stun,
    StaminaRegen
}

public class StatusEffectInstance
{
    public StatusEffectType type;
    public int turnsRemaining;
    public float value;

    public StatusEffectInstance(StatusEffectType type, int turns, float value)
    {
        this.type = type;
        this.turnsRemaining = turns;
        this.value = value;
    }
}
