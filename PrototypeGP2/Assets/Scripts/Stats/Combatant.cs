using System;
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

    public bool IsDead => currentHealth <= 0;

    public event Action<int, int> OnStaminaChanged;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    public virtual void TakeDamage(float damage)
    {
        damage = Mathf.Max(0, damage);
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
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

    public virtual float DealDamage()
    {
        return strength;
    }

    public virtual void Victory()
    {
        Debug.Log("Victory");
    }
}
