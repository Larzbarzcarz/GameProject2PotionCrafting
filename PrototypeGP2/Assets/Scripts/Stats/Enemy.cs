using System.Collections.Generic;
using UnityEngine;

public enum EnemyAction
{
    NormalAttack,
    StrongAttack,
    Buff
}

public class Enemy : Combatant
{
    public enum EnemyState
    {
        Alive,
        Dying,
        Dead
    }
    
    public EnemyState currentState = EnemyState.Alive;

    [Header("Behavior")]
    public List<EnemyAction> actionPattern = new List<EnemyAction>();
    private int currentPatternIndex = 0;
    private bool isBuffed = false;

    public bool IsBuffed => isBuffed;

    protected override void Die()
    {
        currentState = EnemyState.Dead;
        Debug.Log("Enemy defeated!");
    }

    public EnemyAction GetNextAction()
    {
        if (actionPattern == null || actionPattern.Count == 0)
        {
            return EnemyAction.NormalAttack;
        }

        EnemyAction action = actionPattern[currentPatternIndex];
        currentPatternIndex = (currentPatternIndex + 1) % actionPattern.Count;
        return action;
    }

    public void ApplyBuff()
    {
        isBuffed = true;
    }

    public float CalculateDamage(EnemyAction actionType)
    {
        float baseDamage = strength;
        float multiplier = 1.0f;

        if (actionType == EnemyAction.StrongAttack)
        {
            multiplier *= 1.5f; // Strong attack logic
        }

        if (isBuffed)
        {
            multiplier *= 1.3f; // 30% increase from buff
            isBuffed = false;   // Buff consumed
        }

        // Add variety (+/- 10%)
        float variance = Random.Range(0.9f, 1.1f);
        
        return baseDamage * multiplier * variance;
    }

    public override float DealDamage()
    {
        // Legacy support if something still calls this directly
        return CalculateDamage(EnemyAction.NormalAttack);
    }
    public override void TakeDamage(float damage)
    {
      
    }
    
}

