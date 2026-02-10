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

    public override void ResetHealth()
    {
        base.ResetHealth();
        currentState = EnemyState.Alive;
        Debug.Log("enemy revived?");
    }
    
    protected override void Die()
    {
        isDead = true;
        currentState = EnemyState.Dead;
        Debug.Log("enemy defeated?");
    
    }

    public override float DealDamage()
    {
        return strength; 
    }
   
    
}

