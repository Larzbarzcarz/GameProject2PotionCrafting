using UnityEngine;

public class Enemy : Combatant
{
    public bool isDead = false;
    public Animator Animator;

    //-----fmod implementation-----
    private MonsterSound monsterSounds;
    //-----fmod implementation-----

    public void Start()
    {
        monsterSounds = GetComponent<MonsterSound>();
    }

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

        monsterSounds.PlayMonsterSoundDead();

    }

    public override float DealDamage()
    {
        Animator.SetTrigger("Attack");
        monsterSounds.PlayMonsterSoundHit();

        return strength; 
    }

    public override void TakeDamage(float damage)
    {
        Animator.SetTrigger("Damaged");
    }
   
    
}

