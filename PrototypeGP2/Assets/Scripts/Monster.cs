using System;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float Strength;
    [SerializeField] public float Agility;
    [SerializeField] public float maxHealth;
    [SerializeField] public float CurrentHealth;


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
    

    
    
    
    
    
    
}
