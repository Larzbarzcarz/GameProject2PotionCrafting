using UnityEngine;

public class Enemy : Combatant
{
    public enum EnemyState
    {
        Alive,
        Dying,
        Dead
    }
    
    public EnemyState currentState = EnemyState.Alive;
    
    protected override void Die()
    {
        currentState = EnemyState.Dead;
        Debug.Log("Enemy defeated!");
    
    }

    public override float DealDamage()
    {
        return strength; 
    }
}

