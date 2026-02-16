using UnityEngine;

public class Enemy : Combatant
{
    public bool isDead = false;

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
        monsterSounds.PlayMonsterSoundHit();

        return strength; 
    }
   
    
}

