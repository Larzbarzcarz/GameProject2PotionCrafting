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
    public Combatant monster;
    public Combatant enemy;
    public RandomMonsterSpawn monsterSpawner;
    public Transform enemySpawnPoint;
   
    public TextMeshProUGUI dialogueText;
    public GameObject inventory;
    [SerializeField] private DisplayInventory displayInventory;
 
    [Header("Costs")]
    [SerializeField] private int attackCost = 2;
    [SerializeField] private int defendCost = 1;
 
    private PlayerAction selectedAction = PlayerAction.None;
    private bool isDefending;
    private bool battleOver;
 
    public event System.Action OnBattleWon;
    public event System.Action OnBattleLost;

    private void Start()
    {
        inventory.SetActive(false);
    }

    private void Update()
    {
        // debug instakill button
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            if (enemy != null && !enemy.isDead)
            {
                Debug.Log("DEBUG INSTA KILL");
                enemy.TakeDamage(9999f);
            }
        }

        // debug fill pocket with potions
        if (Input.GetKeyDown(KeyCode.Home))
        {
            FillPotionsDebug();
        }
    }

    private void FillPotionsDebug()
    {
        if (displayInventory == null || displayInventory.inventory == null || displayInventory.inventory.database == null)
        {
            Debug.LogWarning("[DEBUG] DisplayInventory reference missing! Searching in scene...");
            displayInventory = FindObjectOfType<DisplayInventory>();
        }

        if (displayInventory == null || displayInventory.inventory == null || displayInventory.inventory.database == null)
        {
            Debug.LogError("DisplayInventory or InventoryObject or Database missing for Potion Refill!");
            return;
        }

        var database = displayInventory.inventory.database;
        var potions = new System.Collections.Generic.List<ItemScriptableObject>();

        foreach (var item in database.Items)
        {
            if (item != null && item.itemType == ItemType.Potion)
            {
                potions.Add(item);
            }
        }

        if (potions.Count == 0)
        {
            Debug.LogWarning("No potions found in database!");
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            var randomPotion = potions[Random.Range(0, potions.Count)];
            displayInventory.inventory.AddItem(new Item(randomPotion), 1, "");
        }

        displayInventory.Refresh();
        Debug.Log("DEBUG FILL POCKET");
    }

    public void BeginBattle()
    {
        battleOver = false;
        
        if (enemy == null && monsterSpawner != null)
        {
             if (enemySpawnPoint == null)
             {
                 Debug.LogError("[BattleSystem] enemySpawnPoint is MISSING in BeginBattle!");
                 return;
             }

             enemy = monsterSpawner.SpawnUniqueMonster(enemySpawnPoint.position);
        }

        if (enemy == null)
        {
             Debug.LogError("[BattleSystem] Enemy failed to spawn/reference in BeginBattle!");
        }
        else
        {
            BindHealthBars();
        }

        _ = StartBattleAsync();
    }

    public void ResetForNewEncounter()
    {
        if (enemy != null)
        {
            Debug.Log("[BattleSystem] Destroying old enemy...");
            Destroy(enemy.gameObject);
            enemy = null;
        }

        if (monster != null)
        {
            monster.ResetAnimator();
        }

        if (monsterSpawner == null)
        {
            Debug.LogError("[BattleSystem] MonsterSpawner is NULL in ResetForNewEncounter!");
            return;
        }

        if (enemySpawnPoint == null)
        {
            Debug.LogError("[BattleSystem] enemySpawnPoint is NULL or was DESTROYED! Make sure it's not a child of an enemy.");
            return;
        }

        Debug.Log("[BattleSystem] Spawning new enemy for encounter...");
        enemy = monsterSpawner.SpawnUniqueMonster(enemySpawnPoint.position);
        
        if (enemy != null)
        {
            BindHealthBars();
        }
    }

    private void BindHealthBars()
    {
        var healthbars = FindObjectsOfType<CombatantHealthbar>();
        foreach (var hb in healthbars)
        {
            if (hb.name.Contains("Monster") || hb.name.Contains("Player")) hb.Bind(monster);
            if (hb.name.Contains("Enemy") || hb.name.Contains("Target")) hb.Bind(enemy);
        }
    }

    private async Task StartBattleAsync()
    {
      
        await Wait(1000);
 
        await DoBattleLoop();
        EndBattle();
    }
 
    
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
            if (monster != null) monster.Victory();
            OnBattleWon?.Invoke();
        }
        else
        {
            dialogueText.text = "You Lose!";
            OnBattleLost?.Invoke();
        }
    }
 
    #region UI
 
    public void OnAttackButton()
    {
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
            if (enemy != null && enemy.isDead) return;
            await Task.Yield();
        }
    }
 
    public void RunAway()
    {
        SceneManager.LoadSceneAsync(0);
    }
 
    #endregion
}