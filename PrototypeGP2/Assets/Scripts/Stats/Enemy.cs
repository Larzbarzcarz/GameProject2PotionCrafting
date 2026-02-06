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

    public EnemyAction GetNextAction()
    {
        if (actionPattern == null || actionPattern.Count == 0)
        {
            Debug.LogWarning($"Enemy on {gameObject.name} has no actionPattern assigned! Defaulting to NormalAttack.");
            return EnemyAction.NormalAttack;
        }

        EnemyAction action = actionPattern[currentPatternIndex];
        Debug.Log($"Enemy on {gameObject.name} selecting action from pattern index {currentPatternIndex}: {action}");
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

        if (isBuffed)
        {
            multiplier *= 1.8f;
            isBuffed = false;
        }

        float variance = UnityEngine.Random.Range(0.9f, 1.1f);

        return baseDamage * multiplier * variance;
    }

    public override float DealDamage()
    {
        return strength; 
    }
    public override void TakeDamage(float damage)
    {
      
    }

    public override void ResetHealth()
    {
        base.ResetHealth();
        currentPatternIndex = 0;
        isBuffed = false;
        currentState = EnemyState.Alive;
        Debug.Log($"[Enemy] Reset for new encounter. Pattern index: 0, Buffed: false");
    }

}

