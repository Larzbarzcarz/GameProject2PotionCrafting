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
    [SerializeField] private InventoryObject playerInventory;
    public InventoryObject PlayerInventory => playerInventory;
    [SerializeField] private DisplayInventory displayInventory;
    [SerializeField] private PotionVariantRegistry potionVariantRegistry;
 
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

        if (playerInventory == null && displayInventory != null)
            playerInventory = displayInventory.inventory;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            if (enemy != null && !enemy.isDead)
            {
                enemy.TakeDamage(9999f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Home))
        {
            FillPotionsDebug();
        }

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            ConsumeRandomPotionDebug();
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            MutationData.LogStatus();
        }

        if (Input.GetKeyDown(KeyCode.End))
        {
            MutationData.Reset();
        }
    }

    private void FillPotionsDebug()
    {
        if (playerInventory == null || playerInventory.database == null)
        {
            Debug.LogError("[DEBUG] PlayerInventory or Database missing for Potion Refill!");
            return;
        }

        var database = playerInventory.database;
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
            var randomPotionSO = potions[Random.Range(0, potions.Count)];
            
            string randomMain = ((MainKeyword)Random.Range(0, System.Enum.GetValues(typeof(MainKeyword)).Length)).ToString();
            string randomBase = ((BaseKeyword)Random.Range(0, System.Enum.GetValues(typeof(BaseKeyword)).Length)).ToString();
            string vKey = $"{randomMain}_{randomBase}";

            playerInventory.AddItem(new Item(randomPotionSO, vKey), 1, vKey);
        }

        if (displayInventory != null) displayInventory.Refresh();
    }

    private void ConsumeRandomPotionDebug()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("[DEBUG] playerInventory missing for potion consumption!");
            return;
        }

        var inv = playerInventory;
        InventorySlot potionSlot = null;

        foreach (var slot in inv.Container.Items)
        {
            if (inv.database.GetItemByStableId.TryGetValue(slot.item.StableId, out var itemSO))
            {
                if (itemSO.itemType == ItemType.Potion && slot.amount > 0)
                {
                    potionSlot = slot;
                    break;
                }
            }
        }

        if (potionSlot == null)
        {
            Debug.LogWarning("[DEBUG] No potions in inventory to consume!");
            return;
        }

        string variantKey = potionSlot.item.VariantKey;
        string potionName = potionSlot.item.Name;
 
        if (MutationRegistry.Instance != null)
        {
            MutationRegistry.Instance.OnPotionConsumed(variantKey);
        }
        if (MutationManager.Instance != null)
        {
            MutationManager.Instance.RecordPotionConsumption(potionSlot.item);
        }
        
        // Try to get BaseKeyword from variant registry first
        BaseKeyword baseKeyword;
        bool foundKeyword = false;

        if (potionVariantRegistry != null && potionVariantRegistry.TryGet(variantKey, out var brewResult))
        {
            baseKeyword = brewResult.baseKeyword;
            foundKeyword = true;
            Debug.Log($"[DEBUG] Found BaseKeyword from registry: {baseKeyword}");
        }
        else if (!string.IsNullOrEmpty(variantKey) && variantKey.Contains("_"))
        {
            // Parse from variantKey format "MainKeyword_BaseKeyword"
            string[] parts = variantKey.Split('_');
            if (parts.Length >= 2 && System.Enum.TryParse(parts[1], out baseKeyword))
            {
                foundKeyword = true;
                Debug.Log($"[DEBUG] Parsed BaseKeyword from variantKey: {baseKeyword}");
            }
            else
            {
                Debug.LogWarning($"[DEBUG] Could not parse BaseKeyword from variantKey: {variantKey}");
                baseKeyword = default;
            }
        }
        else
        {
            Debug.LogWarning($"[DEBUG] No variantKey or registry entry for potion: {potionName}");
            baseKeyword = default;
        }

        if (foundKeyword)
        {
            if (MutationRegistry.Instance != null)
            {
                MutationRegistry.Instance.OnPotionConsumed(baseKeyword);
            }
            else
            {
                MutationData.IncrementKeyword(baseKeyword, out int newTier);
            }
        }

        // Remove potion from inventory
        potionSlot.amount--;
        if (potionSlot.amount <= 0)
        {
            inv.Container.Items.Remove(potionSlot);
        }

        if (displayInventory != null) displayInventory.Refresh();
        Debug.Log($"[DEBUG] Potion consumed. Remaining in inventory: {inv.Container.Items.Count} slots");
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
        if (MutationManager.Instance != null && MutationManager.Instance.ShouldMonsterSkipTurn())
        {
            dialogueText.text = "You are stunned!";
            await Wait(1000);
            return;
        }

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
            float damage = 10f; 

            if (MutationManager.Instance != null)
            {
                damage = MutationManager.Instance.ProcessOutgoingDamage(damage, enemy);
            }

            enemy.TakeDamage(damage);
            dialogueText.text = "Powerful strike!";

            if (MutationManager.Instance != null)
            {
                MutationManager.Instance.OnAfterMonsterAttack(this, enemy, damage);
            }
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
            monster.TakeDamage(8f); // Buffed from 2f
        }
 
        await Wait(1000);
    }
 
    private async Task EnemyTurn()
    {
        dialogueText.text = "Enemy attacks!";
        await Wait(1000);
 
        float damage = isDefending ? 4f : 12f; // Buffed from 1f/2f to 4f/12f
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