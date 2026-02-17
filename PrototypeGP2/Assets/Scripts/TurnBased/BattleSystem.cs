using _Project._Scripts.Sound_and_Music;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private InventoryUI displayInventory;

    [Header("Costs")]
    [SerializeField] private int attackCost = 2;
    [SerializeField] private int defendCost = 1;

    private PlayerAction selectedAction = PlayerAction.None;
    private bool isDefending;
    private bool battleOver;

    public event System.Action OnBattleWon;
    public event System.Action OnBattleLost;
    [Header("Inventory")]
    public InventoryObject PlayerInventory;

    //-----UI-----
    [Header("UI Elements")]
    public GameObject ClawImage;

    [Header("Spawn")]
    public Camera BattleCamera;


    public void SpawnInventoryItem(ItemScriptableObject itemSO)
    {
        if (itemSO == null)
        {
            Debug.LogWarning("Item is null");
            return;
        }

        if (itemSO.worldPrefab == null)
        {
            Debug.LogWarning("World prefab missing on: " + itemSO.name);
            return;
        }
        if (BattleCamera == null)
            BattleCamera = Camera.main;

        float spawnDistance = 1f;
        Vector3 spawnPosition = BattleCamera.transform.position + BattleCamera.transform.forward * spawnDistance;

        spawnPosition += new Vector3(0, -0.5f, 0);



        Instantiate(
            itemSO.worldPrefab,
            spawnPosition,
            Quaternion.identity
        );

        Debug.Log("Spawned near orthographic camera: " + itemSO.name);
    }
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

        // OLD debug fill pocket — replaced by ItemDebugManager (F3)
        // if (Input.GetKeyDown(KeyCode.Home))
        // {
        //     FillPotionsDebug();
        // }

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

    // OLD debug fill pocket — replaced by ItemManager + ItemDebugManager
    /*
    private void FillPotionsDebug()
    {
        if (displayInventory == null || displayInventory.inventory == null || displayInventory.inventory.database == null)
        {
            Debug.LogWarning("[DEBUG] DisplayInventory reference missing! Searching in scene...");
            displayInventory = FindObjectOfType<InventoryUI>();
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
    */

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
            // ── process status effects at start of round ──
            if (monster != null && !monster.isDead)
                monster.ProcessStatusEffects();
            if (enemy != null && !enemy.isDead)
                enemy.ProcessStatusEffects();

            // check if DoT killed anyone
            if (enemy != null && enemy.isDead) { battleOver = true; return; }
            if (monster != null && monster.isDead) { battleOver = true; return; }

            // ── monster turn (skip if stunned) ──
            if (monster.IsStunned())
            {
                dialogueText.text = "Your monster is stunned!";
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

            // ── enemy turn (skip if stunned) ──
            if (enemy.IsStunned())
            {
                dialogueText.text = "The enemy is stunned!";
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

        if (hitChance < 100)
        {
            //-----fmod implementation-----
            int randomAttackSound = Random.Range(1, 3);
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
            }

            ClawImage.SetActive(true);
            await Wait(500);
            ClawImage.SetActive(false);
            //-----fmod implementation-----

            // apply damage with boost and defence modifiers
            float baseDamage = Random.Range(2, 4);
            float boost = monster.GetDamageBoost();
            float totalDamage = baseDamage + boost;
            float defMod = enemy.GetDefenceModifier();
            float finalDamage = Mathf.Max(0, totalDamage - Mathf.Max(0, defMod));
            enemy.TakeDamage(finalDamage);
            Debug.Log($"[Battle] Monster attack: base={baseDamage}, boost={boost}, enemyDef={defMod}, final={finalDamage}");

            // damage reflect
            float reflect = enemy.GetDamageReflect();
            if (reflect > 0)
            {
                float reflected = finalDamage * reflect;
                monster.TakeDamage(reflected);
                Debug.Log($"[Battle] Damage reflected back: {reflected}");
            }

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

        // apply damage with boost and defence modifiers
        float baseDamage = isDefending ? 1f : 2f;
        float boost = enemy.GetDamageBoost();
        float totalDamage = baseDamage + boost;
        float defMod = monster.GetDefenceModifier();
        float finalDamage = Mathf.Max(0, totalDamage - Mathf.Max(0, defMod));
        monster.TakeDamage(finalDamage);
        Debug.Log($"[Battle] Enemy attack: base={baseDamage}, boost={boost}, monsterDef={defMod}, final={finalDamage}");

        // damage reflect
        float reflect = monster.GetDamageReflect();
        if (reflect > 0)
        {
            float reflected = finalDamage * reflect;
            enemy.TakeDamage(reflected);
            Debug.Log($"[Battle] Damage reflected back to enemy: {reflected}");
        }

        //-----fmod implementation-----
        enemy.DealDamage(); // This will trigger the enemy's attack sound

        await Wait(600);

        int randomHurt = Random.Range(1, 3);
        switch (randomHurt)
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
        }

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
            AudioManager.Instance.PlayOneShotAtPosition(FMODEvents.instance.victory, monster.transform.position);
            AudioManager.Instance.PlayOneShotAtPosition(FMODEvents.instance.victoryMusic, monster.transform.position);

        }
        else
        {
            dialogueText.text = "You Lose!";
            OnBattleLost?.Invoke();
            AudioManager.Instance.PlayOneShot(FMODEvents.instance.loss);
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

    public void InventoryActive()
    {
        inventory.SetActive(true);
    }

    public void InventoryInactive()
    {
        inventory.SetActive(false);
    }
}