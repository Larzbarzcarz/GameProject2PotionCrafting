using UnityEngine;

public class Monster : Combatant
{
    public enum MonsterState
    {
        Alive,
        Dying,
        Dead
    }

    public MonsterState currentState = MonsterState.Alive;

    protected override void Die()
    {
        currentState = MonsterState.Dead;
        Debug.Log("Monster died!");
    }

    public override float DealDamage()
    {
        return strength * 1.2f; 
    }
}


