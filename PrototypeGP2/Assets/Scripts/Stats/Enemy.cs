using UnityEngine;

public class Enemy : Combatant
{
    public bool isDead = false;
    public enum EnemyState
    {
        Alive,
        Dying,
        Dead
    }
    
    public EnemyState currentState = EnemyState.Alive;
    
    protected override void Die()
    {
        isDead = true;
        currentState = EnemyState.Dead;
        Debug.Log("Enemy defeated!");
    
    }

    public override float DealDamage()
    {
        return strength; 
    }
   
    
}

