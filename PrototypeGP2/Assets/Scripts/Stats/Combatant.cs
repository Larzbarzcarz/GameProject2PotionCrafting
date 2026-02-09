using System;
using UnityEngine;



public abstract class Combatant : MonoBehaviour, IPotionTarget
{
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
	
	public virtual void Dying()
{

}
	
}
