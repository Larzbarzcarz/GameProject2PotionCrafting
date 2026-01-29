using System;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Monster Stats")]
    [SerializeField] public float   speed;
    [SerializeField] public float   Strength;
    [SerializeField] public float   Agility;
    [SerializeField] public float   maxHealth;
    [SerializeField] public float   CurrentHealth;
    [SerializeField] public int     maxStamina;
    [SerializeField] public int     currentStamina;

    public int MaxStamina => maxStamina;
    public int CurrentStamina => currentStamina;

    public event Action<int, int> OnStaminaChanged;

    public void Awake()
    {
        
        
        CurrentHealth = maxHealth;
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(CurrentHealth);
            TakeDamage(2f);
        }
    }
    
    public void TakeDamage(float damage)
    {
        damage = Mathf.Clamp(damage, 2f, maxHealth);
        
        CurrentHealth -= damage;
    }
    public float Damage(float damage)
    {
        damage = 3f;
        return damage;
    }
    
    public void SetMaxStamina(int newMax, bool fillToMax = false)
    {
        maxStamina = Mathf.Max(1, newMax);
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (fillToMax)
            currentStamina = maxStamina;

        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    public bool TrySpendStamina(int cost)
    {
        if (!SpendStamina(cost))
            return false;

        currentStamina -= cost;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        return true;
    }

    public bool SpendStamina(int cost)
    {
        if (cost < 0)
            cost = 0;
        return currentStamina >= cost;
    }
}
