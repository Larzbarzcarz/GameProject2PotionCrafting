using _Project._Scripts.Sound_and_Music;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public enum PlayerAction
{
    None,
    Attack,
    Defend,
    Potion
}
 
public class BattleSystem : MonoBehaviour
{
    [Header("References")] 
    public Combatant monster;
    public Combatant enemy;
    public RandomMonsterSpawn monsterSpawner;
    public Transform enemySpawnPoint;
    public PotionVariantRegistry variantRegistry;
   
    public TextMeshProUGUI dialogueText;
    public GameObject inventory;
    [SerializeField] private PotionDisplayInventory displayInventory;
 
    [Header("Costs")]
    [SerializeField] private int attackCost = 2;
    [SerializeField] private int defendCost = 1;
    [SerializeField] private int potionCost = 1;
 
    private PlayerAction selectedAction = PlayerAction.None;
    private Item selectedPotion = null;
    private bool isDefending;
    private bool battleOver;
 
    public event System.Action OnBattleWon;
    public event System.Action OnBattleLost;
    [Header("Inventory")]
    public InventoryObject PlayerInventory;

    //-----UI-----
    [Header("UI Elements")]
    public GameObject ClawImage;

    private void Start()
    {
        inventory.SetActive(false);
        if (variantRegistry == null) variantRegistry = FindObjectOfType<PotionVariantRegistry>();
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

        // debug suicide button
        if (Input.GetKeyDown(KeyCode.End))
        {
            if (monster != null && !monster.isDead)
            {
                Debug.Log("DEBUG SUICIDE");
                monster.TakeDamage(9999f);
            }
        }
    }

    private void FillPotionsDebug()
    {
        if (displayInventory == null || displayInventory.inventory == null || displayInventory.inventory.database == null)
        {
            Debug.LogWarning("[DEBUG] DisplayInventory reference missing! Searching in scene...");
            displayInventory = FindObjectOfType<PotionDisplayInventory>();
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
            monster.ResetHealth(); // Clear effects as well
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
            monster.ProcessTurnEffects();
            if (monster.isDead) { battleOver = true; return; }
            if (monster.isStunned)
            {
                dialogueText.text = "Monster is stunned!";
                await Wait(1000);
            }
            else
            {
                await MonsterTurn();
            }
 
            if (enemy.isDead)
            {
                battleOver = true;
                return;
            }

            enemy.ProcessTurnEffects();
            if (enemy.isDead) { battleOver = true; return; }
            if (enemy.isStunned)
            {
                dialogueText.text = "Enemy is stunned!";
                await Wait(1000);
            }
            else
            {
                await EnemyTurn();
            }
 
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
        selectedPotion = null;
 
        await WaitUntilActionSelected();
 
        switch (selectedAction)
        {
            case PlayerAction.Attack:
                await MonsterAttack();
                break;
 
            case PlayerAction.Defend:
                await MonsterDefend();
                break;
            case PlayerAction.Potion:
                await ExecutePotionUsage();
                break;
        }
    }
 
    private async Task MonsterAttack()
    {
        int hitChance = Random.Range(0, 100);
 
        if (hitChance < 100)
        {
            //-----fmod implementation-----
            /*int randomAttackSound = Random.Range(1, 3);
            switch (randomAttackSound)
            {
                case 1:
                    AudioManager.Instance.PlayOneShotAtPosition(FMODEvents.instance.playerAttack1, monster.transform.position);
                    break;
                case 2:
                    AudioManager.Instance.PlayOneShotAtPosition(FMODEvents.instance.playerAttack2, monster.transform.position);
                    break;
                case 3:
                    AudioManager.Instance.PlayOneShotAtPosition(FMODEvents.instance.playerAttack3, monster.transform.position);
                    break;
            }*/

            ClawImage.SetActive(true);
            await Wait(500);
            ClawImage.SetActive(false);

            int randomAttack = Random.Range(2, 4);
            enemy.TakeDamage(randomAttack);
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

    private async Task ExecutePotionUsage()
    {
        if (selectedPotion == null) return;

        if (variantRegistry.TryGet(selectedPotion.VariantKey, out var result))
        {
            dialogueText.text = $"Using {selectedPotion.Name}!";
            ApplyPotionResult(result);
            MutationManager.Instance?.RecordPotionConsumption(selectedPotion);
        }
        else
        {
            dialogueText.text = "The potion has no effect...";
        }

        await Wait(1000);
    }

    private void ApplyPotionResult(BrewResult result)
    {
        Combatant target = result.effect == PotionEffectType.Damage ? enemy : monster;

        // Instant effects
        if (result.instant)
        {
            if (result.percentOfMaxHP > 0) target.Heal(target.MaxHealth * result.percentOfMaxHP);
            if (result.damage > 0) target.TakeDamage(result.damage);
            if (result.multiplier > 0) target.HealStamina(result.multiplier * 2);
        }

        // Over-time effects
        if (result.turns > 0)
        {
            if (result.percentOfMaxHP > 0) target.ApplyStatusEffect(StatusEffectType.Regen, result.turns, result.percentOfMaxHP / result.turns);
            if (result.damage > 0) target.ApplyStatusEffect(StatusEffectType.Poison, result.turns, result.damage / result.turns);
            if (result.multiplier > 0) target.ApplyStatusEffect(StatusEffectType.StaminaRegen, result.turns, result.multiplier);
        }

        // Special: Stun
        if (result.effect == PotionEffectType.Utility && result.turns > 0)
        {
            enemy.ApplyStatusEffect(StatusEffectType.Stun, result.turns, 0);
        }
    }
 
    private async Task EnemyTurn()
    {
        dialogueText.text = "Enemy attacks!";
        await Wait(1000);
 
        float damage = isDefending ? 1f : 2f;
        monster.TakeDamage(damage);

        enemy.DealDamage(); 

        await Wait(600);

        // int randomHurt = Random.Range(1, 3);
        /*switch (randomHurt)
        {
            case 1:
                AudioManager.Instance.PlayOneShotAtPosition(FMODEvents.instance.playerHurt1, monster.transform.position);
                break;
            case 2:
                AudioManager.Instance.PlayOneShotAtPosition(FMODEvents.instance.playerHurt2, monster.transform.position);
                break;
            case 3:
                AudioManager.Instance.PlayOneShotAtPosition(FMODEvents.instance.playerHurt3, monster.transform.position);
                break;
        }*/

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

    public void OnPotionSelected(Item potion)
    {
        if (selectedAction != PlayerAction.None) return;

        if (!monster.TrySpendStamina(potionCost))
        {
            dialogueText.text = "Not enough stamina to use a potion!";
            return;
        }

        selectedPotion = potion;
        selectedAction = PlayerAction.Potion;
        InventoryInactive();
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
 
    public void InventoryActive()
    {
        inventory.SetActive(true);
    }
 
    public void InventoryInactive()
    {
        inventory.SetActive(false);
    }
}
