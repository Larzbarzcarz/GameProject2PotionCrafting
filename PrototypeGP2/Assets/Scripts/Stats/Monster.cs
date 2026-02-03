using UnityEngine;

public class Monster : Combatant
{
    public Animator Animator;
    bool victory = false;
   
    public bool Dead;
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
        Debug.Log("Dealing");
        
        Animator.SetTrigger("Attack");
      
       
        return strength * 1.2f; 
    }

    public override void TakeDamage(float damage)
    {
        Debug.Log("Taking damage");
        Animator.SetTrigger("Damaged");
        
    }

    public override void Victory()
    {
        victory = true;
        Animator.SetTrigger("Victory");
        
    }

    
    
}


