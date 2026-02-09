using UnityEngine;

public class Monster : Combatant
{
    public Animator Animator;
    bool victory = false;
	bool dying = false;

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
        float damage = strength * 1.2f;
        Debug.Log($"Monster dealing {damage} damage (Strength: {strength} * 1.2).");
        Animator.SetTrigger("Attack");


        return strength * 1.2f;
    }

    public override void TakeDamage(float damage)
    {
        Debug.Log("Taking damage");
        Animator.SetTrigger("Damaged");
        base.TakeDamage(damage);
    }

    private void Update()
    {
        // debugging victory animation
        if (victory && Animator != null && Time.frameCount % 60 == 0)
        {
            var state = Animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log($"[MONSTER DEBUG] checking victory mode - animator state hash: {state.fullPathHash} - loop: {state.loop}");
        }
    }

    public override void Victory()
    {
        Debug.Log("victory called, triggering dance?");
        victory = true;
        if (Animator != null) Animator.SetTrigger("Victory");
    }

    public void ResetAnimator()
    {
        Debug.Log("rebinding");
        victory = false;
        if (Animator != null)
        {
            Animator.Rebind();
            Animator.Update(0f);
            
            var state = Animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log($"[Monster] Post-Rebind state Hash: {state.fullPathHash}");
        }
    }

	public override void Dying()
	{
		//currentState = MonsterState.Dying;
		//Animator.SetBool("Dying");
    }



}


