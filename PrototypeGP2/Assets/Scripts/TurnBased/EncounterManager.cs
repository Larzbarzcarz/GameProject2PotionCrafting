using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class EncounterManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int labSceneIndex = 0;

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

        // auto-fill pocket from stash when entering combat scene
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.FillPocketFromStash();
        }
        else
        {
            Debug.LogWarning("[EncounterManager] ItemManager not found! Pocket will be empty.");
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

        if (combatUI == null)
        {
            Debug.LogWarning("[EncounterManager] combatUI is NOT assigned in the Inspector! Please drag 'Fight_UI (1)' into the Combat UI slot.");
        }

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
            // expedition complete - grant rewards
            GrantRewards();

            if (combatUI != null) combatUI.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(true);
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

        // separate by rarity
        List<IngredientObject> common = allIngredients.FindAll(x => x.rarity == Rarity.Common);
        List<IngredientObject> rare = allIngredients.FindAll(x => x.rarity == Rarity.Rare);
        List<IngredientObject> legendary = allIngredients.FindAll(x => x.rarity == Rarity.Legendary);

        // fallback if rarity not fully assigned yet
        if (common.Count == 0 && rare.Count == 0 && legendary.Count == 0)
        {
            common = allIngredients; // treat all as common if none are assigned
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

            // fallbacks
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

        // sort by rarity for display niceness? (optional)
        return generatedRewards;
    }

    private void HandleBattleLost()
    {
        Debug.Log("[EncounterManager] Battle Lost.");
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
