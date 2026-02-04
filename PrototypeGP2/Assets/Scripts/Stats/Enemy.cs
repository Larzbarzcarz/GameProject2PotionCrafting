using System.Collections.Generic;
using UnityEngine;

public enum EnemyAction
{
    NormalAttack,
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
	public Animator anim;

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

        float variance = Random.Range(0.9f, 1.1f);

        return baseDamage * multiplier * variance;
    }

    public override float DealDamage()
    {
        return CalculateDamage(EnemyAction.NormalAttack);
    }
    public override void TakeDamage(float damage)
    {
		
		anim.SetTrigger("Damaged");
		Debug.Log("Damaged animation playing" + transform);
        Debug.Log($"Enemy taking {damage} damage. Current Health: {currentHealth} -> {currentHealth - damage}");
        base.TakeDamage(damage);
        if (currentHealth <= 0) Die();
    }

}

