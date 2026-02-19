using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class EncounterManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int labSceneIndex = 1;

    [Header("References")]
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject intermissionPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject combatUI;

    [Header("Reward System")]
    [SerializeField] private ItemDatabaseObject itemDatabase;
    [SerializeField] private InventoryObject playerInventory;
    [SerializeField] private ExpeditionRewardUI expeditionRewardUI;

    private void Update()
    {
        // SANITY SAVER: Press F9 to instantly trigger the Victory Screen and Rewards for testing
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Debug.Log("[EncounterManager] Debug Victory Triggered (F9)");
            ExpeditionData.CurrentEncounterIndex = ExpeditionData.TotalEncounters;
            HandleBattleWon();
        }
    }

    private void Start()
    {
        if (!ExpeditionData.IsExpeditionActive)
        {
            ExpeditionData.StartNewExpedition();
            if (battleSystem != null && battleSystem.monsterSpawner != null)
            {
                battleSystem.monsterSpawner.ResetHistory();
            }
        }

        if (battleSystem != null)
        {
            battleSystem.OnBattleWon += HandleBattleWon;
            battleSystem.OnBattleLost += HandleBattleLost;

            StartEncounter();
        }
        else
        {
            Debug.LogError("[EncounterManager] BattleSystem reference is missing!");
        }

        if (victoryPanel) victoryPanel.SetActive(false);
        if (intermissionPanel) intermissionPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (battleSystem != null)
        {
            battleSystem.OnBattleWon -= HandleBattleWon;
            battleSystem.OnBattleLost -= HandleBattleLost;
        }
    }

    private void StartEncounter()
    {
        Debug.Log($"[EncounterManager] Starting encounter {ExpeditionData.CurrentEncounterIndex}/{ExpeditionData.TotalEncounters}");

        if (combatUI != null) combatUI.SetActive(true);

        if (battleSystem != null)
        {
            battleSystem.ResetForNewEncounter();
            battleSystem.BeginBattle();
        }
    }

    private void HandleBattleWon()
    {
        Debug.Log("[EncounterManager] Battle Won!");

        // Ensure all combat-specific HUD is hidden
        if (battleSystem != null) battleSystem.InventoryInactive();

        if (ExpeditionData.CurrentEncounterIndex < ExpeditionData.TotalEncounters)
        {
            if (intermissionPanel != null)
            {
                intermissionPanel.SetActive(true);
            }
            else
            {
                Debug.Log("[EncounterManager] No Intermission Panel. Auto-continuing in 2 seconds...");
                Invoke(nameof(StartNextEncounter), 2.0f);
            }
        }
        else
        {
            // Expedition Complete - Grant Rewards
            GrantRewards();

            // Hide the entire Combat UI root
            if (combatUI != null)
            {
                combatUI.SetActive(false);
            }
            else
            {
                Debug.LogError("[EncounterManager] CRITICAL: 'Combat UI' slot is EMPTY! Drag 'Fight_UI (1)' into this slot to fix the overlap.");
            }

            if (victoryPanel != null)
            {
                // Ensure the Victory parent container (Sucessorfailscreen) is active
                GameObject parentObj = victoryPanel.transform.parent ? victoryPanel.transform.parent.gameObject : null;
                if (parentObj != null) parentObj.SetActive(true);

                victoryPanel.SetActive(true);

                // OBJECTIVE FIX: Recursively enable children (like the inactive VictoryText)
                foreach (Transform child in victoryPanel.GetComponentsInChildren<Transform>(true))
                {
                    child.gameObject.SetActive(true);
                }
            }

            // Ensure the separate Reward UI root is visible
            if (expeditionRewardUI != null)
            {
                expeditionRewardUI.gameObject.SetActive(true);

                // Ensure the Reward UI Canvas is at the VERY FRONT
                Canvas rewardCanvas = expeditionRewardUI.GetComponentInChildren<Canvas>(true);
                if (rewardCanvas != null)
                {
                    rewardCanvas.gameObject.SetActive(true);
                    rewardCanvas.overrideSorting = true;
                    rewardCanvas.sortingOrder = 1000; // Force to absolute front
                    Debug.Log($"[EncounterManager] Forced Reward Canvas '{rewardCanvas.name}' to Sorting Order 1000.");
                }
            }

            ExpeditionData.EndExpedition();
        }
    }

    private void GrantRewards()
    {
        if (itemDatabase == null || playerInventory == null)
        {
            Debug.LogError("[EncounterManager] Database or Inventory not assigned! Cannot grant rewards.");
            return;
        }

        List<Item> rewards = GenerateRewards();

        Debug.Log($"[EncounterManager] Granting {rewards.Count} rewards...");

        // Add to inventory
        foreach (var reward in rewards)
        {
            playerInventory.AddItem(reward, 1, "");
        }

        // Display in UI
        if (expeditionRewardUI != null)
        {
            expeditionRewardUI.DisplayRewards(rewards);
        }

        Debug.Log("[EncounterManager] All rewards granted and added to inventory.");
    }

    private List<Item> GenerateRewards()
    {
        List<Item> generatedRewards = new List<Item>();
        List<IngredientObject> allIngredients = new List<IngredientObject>();

        // Filter ingredients only
        foreach (var item in itemDatabase.Items)
        {
            if (item is IngredientObject ing)
            {
                allIngredients.Add(ing);
            }
        }

        if (allIngredients.Count == 0) return generatedRewards;

        // Separate by rarity
        List<IngredientObject> common = allIngredients.FindAll(x => x.rarity == Rarity.Common);
        List<IngredientObject> rare = allIngredients.FindAll(x => x.rarity == Rarity.Rare);
        List<IngredientObject> legendary = allIngredients.FindAll(x => x.rarity == Rarity.Legendary);

        // Fallback if rarity not fully assigned yet
        if (common.Count == 0 && rare.Count == 0 && legendary.Count == 0)
        {
            common = allIngredients; // Treat all as common if none are assigned
        }

        for (int i = 0; i < 10; i++)
        {
            float roll = Random.value;
            IngredientObject chosen = null;

            if (roll < 0.6f && common.Count > 0) // 60% Common
            {
                chosen = common[Random.Range(0, common.Count)];
            }
            else if (roll < 0.9f && rare.Count > 0) // 30% Rare
            {
                chosen = rare[Random.Range(0, rare.Count)];
            }
            else if (legendary.Count > 0) // 10% Legendary
            {
                chosen = legendary[Random.Range(0, legendary.Count)];
            }

            // Fallbacks
            if (chosen == null)
            {
                if (common.Count > 0) chosen = common[Random.Range(0, common.Count)];
                else if (rare.Count > 0) chosen = rare[Random.Range(0, rare.Count)];
                else if (legendary.Count > 0) chosen = legendary[Random.Range(0, legendary.Count)];
            }

            if (chosen != null)
            {
                generatedRewards.Add(new Item(chosen));
            }
        }

        // Sort by Rarity for display niceness? (Optional)
        return generatedRewards;
    }

    private void HandleBattleLost()
    {
        Debug.Log("[EncounterManager] Battle Lost.");

        // Ensure inventory is hidden
        if (battleSystem != null) battleSystem.InventoryInactive();

        if (combatUI != null) combatUI.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(true);
        ExpeditionData.EndExpedition();
    }

    public void StartNextEncounter()
    {
        if (intermissionPanel != null) intermissionPanel.SetActive(false);

        ExpeditionData.AdvanceEncounter();

        StartEncounter();
    }

    public void ReturnToLab()
    {
        SceneManager.LoadScene(labSceneIndex);
    }
}
