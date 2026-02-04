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

    [Header("Combatants")]
    public Combatant monster;

    public Combatant enemy;



    public TextMeshProUGUI dialogueText;

    public GameObject inventory;

    [Header("Costs")]

    [SerializeField] private int attackCost = 2;

    [SerializeField] private int defendCost = 1;

    private PlayerAction selectedAction = PlayerAction.None;

    private bool isDefending;

    private bool battleOver;

    private void Start()

    {

        inventory.SetActive(false);

        _ = StartBattleAsync();

    }

    private async Task StartBattleAsync()

    {



        await Wait(1000);

        await DoBattleLoop();

        EndBattle();

    }



    private async Task DoBattleLoop()

    {
        Debug.Log("--- Battle Loop Started ---");
        while (!battleOver)

        {
            Debug.Log("--- Monster Turn ---");
            await MonsterTurn();

            if (enemy.IsDead)

            {
                Debug.Log("Enemy detected as dead. Ending loop.");
                battleOver = true;

                return;

            }

            Debug.Log("--- Enemy Turn ---");
            await EnemyTurn();

            if (monster.IsDead)
            {
                Debug.Log("Monster detected as dead. Ending loop.");
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
        Debug.Log($"Monster Attack rolled: {hitChance} (Need < 80)");

        if (hitChance < 80)
        {
            float damage = monster.DealDamage();
            Debug.Log($"Monster Attack HIT for {damage} damage.");
            enemy.TakeDamage(damage);
            dialogueText.text = "The attack hit!";

        }

        else

        {
            Debug.Log("Monster Attack MISSED.");
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
            Enemy enemyStats = enemy as Enemy;
            float damage = enemyStats != null ? enemyStats.CalculateDamage(EnemyAction.NormalAttack) : 2f;
            monster.TakeDamage(damage);

        }

        await Wait(1000);

    }

    private async Task EnemyTurn()
    {
        Enemy enemyStats = enemy as Enemy;
        if (enemyStats == null)
        {
            Debug.Log("EnemyStats is null, performing default attack.");
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
            return;
        }

        EnemyAction action = enemyStats.GetNextAction();
        Debug.Log($"Enemy selected action: {action}");

        switch (action)
        {
            case EnemyAction.Buff:
                enemyStats.ApplyBuff();
                dialogueText.text = "Enemy is focusing...";
                break;

            case EnemyAction.NormalAttack:
                bool wasBuffed = enemyStats.IsBuffed;
                dialogueText.text = wasBuffed ? "Enemy strikes with power!" : "Enemy attacks!";
                await Wait(1000);

                float damage = enemyStats.CalculateDamage(action);
                Debug.Log($"Enemy calculated damage: {damage} (Was Buffed: {wasBuffed})");

                if (isDefending)
                {
                    damage *= 0.5f;
                    Debug.Log($"Damage reduced by defense to: {damage}");
                    dialogueText.text = "Damage reduced!";
                    isDefending = false;
                    await Wait(1000);
                }

                monster.TakeDamage(damage);
                break;
        }

        await Wait(1000);
    }

    private void EndBattle()

    {

        if (enemy.IsDead)

        {

            dialogueText.text = "You Win!";
            monster.Victory();
        }

        else

        {

            dialogueText.text = "You Lose!";

        }

    }

    #region UI

    public void OnAttackButton()

    {
        Debug.Log("Attacking");

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

    // Aliases for scene compatibility
    public void DefenseButton() => OnDefendButton();
    public void Runnaway() => RunAway();

    public void ActivateInventory()
    {
        if (inventory != null) inventory.SetActive(true);
    }

    public void DeactivateInventory()
    {
        if (inventory != null) inventory.SetActive(false);
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
