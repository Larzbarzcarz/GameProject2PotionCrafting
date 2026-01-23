using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Win,
    Lose
}
public class BattleSystem : MonoBehaviour
{
    public GameObject MonsterPrefab;
    public GameObject EnemyPrefab;

    public Monster monster;
    public Enemy _enemy;
    public Transform MonsterBattleStation;
    public Transform EnemyBattleStation;
    
    public BattleState state;
    public TextMeshProUGUI dialogeText;
    
    
    
    void Start()
     { 
         state = BattleState.Start;
        StartCoroutine(SetupBattle());
    }
    
    IEnumerator SetupBattle()
    {
        GameObject MonsterGO = Instantiate(MonsterPrefab, MonsterBattleStation );
        monster =  MonsterGO.GetComponent<Monster>();

        GameObject EnemyGO = Instantiate(EnemyPrefab, EnemyBattleStation);
        _enemy = EnemyGO.GetComponent<Enemy>();

        dialogeText.text = "A wild " + _enemy.enemyName + "approaches";

        yield return new WaitForSeconds(2f);

        state = BattleState.PlayerTurn;
        MonsterTurn();

    }

    IEnumerator MonsterAttack()
    {
        _enemy.TakeDamage(monster.Damage(2f));
        Debug.Log("Dealing Damage");
            dialogeText.text = "The attack hit";
            
            yield return new WaitForSeconds(2f);

            if (_enemy.isDead)
            {
                state = BattleState.Win;
                EndBattle();
            }
            else
            {
                state = BattleState.EnemyTurn;
                StartCoroutine(EnemyTurn());
            }
    }

    IEnumerator EnemyTurn()
    {
        dialogeText.text = "Enemy +  Attacks!";
        yield return new WaitForSeconds(1f);

        if (monster.CurrentHealth == 0)
        {
            state = BattleState.Lose;
            EndBattle();
        }
        else
        {
            state = BattleState.PlayerTurn;
            MonsterTurn();
        }

    }

    void MonsterTurn()
    {
        dialogeText.text = "Select a action";
    }

    public void OnAttackButton()
    {
        if (state == BattleState.PlayerTurn)
        return;

        StartCoroutine(MonsterAttack());
    }

    void EndBattle()
    {
        if (state == BattleState.Win)
        {
            dialogeText.text = "You Win!";
        }
        else if (state == BattleState.Lose)
        {
            dialogeText.text = "You lose";
        }
    }
    
  
   
    


}
