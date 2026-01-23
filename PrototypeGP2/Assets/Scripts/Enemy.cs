using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float Strength;
    [SerializeField] public float Agility;
    [SerializeField] public float maxHealth;
    [SerializeField] public float CurrentHealth;
    public string enemyName;
    

    public EnemyState currentState = EnemyState.Alive;
    public bool isDead => currentState == EnemyState.Dead || CurrentHealth <= 0;
    public bool isAlive => currentState == EnemyState.Alive;
    

    public enum EnemyState
    {
        Alive,
        Dying,
        Dead
    }
    
    
    public void Awake()
    {
        
        CurrentHealth = maxHealth;
        
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
    }
    public void dealDamage(float damage)
    {
        damage = 10f;
        return;
    }
    
    
    
}
