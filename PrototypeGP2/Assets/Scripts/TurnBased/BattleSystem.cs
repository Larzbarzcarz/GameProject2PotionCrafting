using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
 
public enum PlayerAction
{
    None,
    Attack,
    Defend
}
 
public class BattleSystem : MonoBehaviour
{
    [Header("References")] 
    public Combatant Monster;
    public Combatant Enemy:
    public TextMeshProUGUI dialogueText;
    public GameObject inventory;
 
    [Header("Costs")]
    [SerializeField] private int attackCost = 2;
    [SerializeField] private int defendCost = 1;
 
    private PlayerAction selectedAction = PlayerAction.None;
    private bool isDefending;
    private bool battleOver;

    //-----Fmod Integration-----
    private MusicManager Sounds;

    private void Start()
    {
        Sounds = FindFirstObjectByType<MusicManager>();
        inventory.SetActive(false);
        _ = StartBattleAsync();
    }
 
    private async Task StartBattleAsync()
    {
        dialogueText.text = $"A wild {enemy.enemyName} approaches!";
        await Wait(1000);
 
        await DoBattleLoop();
        EndBattle();
    }
 
    /// <summary>
    /// Single source of truth for battle flow
    /// </summary>
    private async Task DoBattleLoop()
    {
        while (!battleOver)
        {
            await MonsterTurn();
 
            if (enemy.isDead)
            {
                battleOver = true;
                return;
            }
 
            await EnemyTurn();
 
            if (monster.CurrentHealth <= 0)
            {
                battleOver = true;
                return;
            }
        }
    }
 
    private async Task MonsterTurn()
    {
        dialogueText.text = "Choose an action";
        selectedAction = PlayerAction.None;
 
        await WaitUntilActionSelected();
 
        switch (selectedAction)
        {
            case PlayerAction.Attack:
                await MonsterAttack();
                break;
 
            case PlayerAction.Defend:
                await MonsterDefend();
                break;
        }
    }
 
    private async Task MonsterAttack()
    {
        int hitChance = Random.Range(0, 100);
 
        if (hitChance < 80)
        {
            enemy.TakeDamage(2f);
            dialogueText.text = "The attack hit!";

            //-----FMOD Integration-----
            //----------------------------------------------------------------------------------------------

        }
        else
        {
            dialogueText.text = "You missed!";
        }
 
        await Wait(1000);
    }
 
    private async Task MonsterDefend()
    {
        int defendChance = Random.Range(0, 100);
 
        if (defendChance < 70)
        {
            isDefending = true;
            dialogueText.text = "You brace for impact";
        }
        else
        {
            dialogueText.text = "You failed to defend!";
            monster.TakeDamage(2f);
        }
 
        await Wait(1000);
    }
 
    private async Task EnemyTurn()
    {
        dialogueText.text = "Enemy attacks!";
        await Wait(1000);
 
        float damage = isDefending ? 1f : 2f;
        monster.TakeDamage(damage);
 
        if (isDefending)
        {
            dialogueText.text = "Damage reduced!";
            isDefending = false;
            await Wait(1000);
        }
    }
 
    private void EndBattle()
    {
        if (enemy.isDead)
        {
            dialogueText.text = "You Win!";
        }
        else
        {
            dialogueText.text = "You Lose!";
        }
    }
 
    #region UI
 
    public void OnAttackButton()
    {
        //-----FMOD Integration-----
        Sounds.PlaySound(0);

        if (selectedAction != PlayerAction.None) return;
 
        if (!monster.TrySpendStamina(attackCost))
        {
            dialogueText.text = "Not enough stamina!";
            return;
        }
 
        selectedAction = PlayerAction.Attack;
    }
 
    public void OnDefendButton()
    {
        if (selectedAction != PlayerAction.None) return;
 
        if (!monster.TrySpendStamina(defendCost))
        {
            dialogueText.text = "Not enough stamina!";
            return;
        }
 
        selectedAction = PlayerAction.Defend;
    }
 
    #endregion
 
    #region Helpers
 
    private async Task Wait(int ms) => await Task.Delay(ms);
 
    private async Task WaitUntilActionSelected()
    {
        while (selectedAction == PlayerAction.None)
        {
            await Task.Yield();
        }
    }
 
    public void RunAway()
    {
        SceneManager.LoadSceneAsync(0);
    }
 
    #endregion
}