using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }

    [Header("Run Config")]
    public string LabSceneName = "Beta Scene";
    public string CombatSceneName = "BetaLevel1";
    public InventoryObject playerInventory;
    public ItemDatabaseObject database;

    [Header("Current Run State")]
    public int currentEncounterIndex = 0;
    public List<InventorySlot> carriedPotions = new List<InventorySlot>();
    public List<Item> sessionLoot = new List<Item>();
    public bool isRunActive = false;

    private bool pendingTreasureDisplay = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pendingTreasureDisplay && scene.name == LabSceneName)
        {
            Debug.Log("Back in Lab with pending treasure. Showing UI...");

            TreasureRoomUI treasureUI = null;
            var results = Resources.FindObjectsOfTypeAll<TreasureRoomUI>();
            if (results.Length > 0) treasureUI = results[0];

            if (treasureUI != null)
            {
                treasureUI.ShowTreasure(sessionLoot);
                pendingTreasureDisplay = false;
            }
            else
            {
                Debug.LogWarning("TreasureRoomUI not found in Lab scene!");
            }
        }
    }

    public void StartRun(List<InventorySlot> selectedPotions)
    {
        Debug.Log("Starting Expedition run...");
        isRunActive = true;
        currentEncounterIndex = 1;
        sessionLoot.Clear();
        carriedPotions = new List<InventorySlot>(selectedPotions);

        foreach (var slot in carriedPotions)
        {
            playerInventory.RemoveItem(slot.item.StableId, slot.amount);
        }

        playerInventory.Save();
        Debug.Log($"Loading combat scene: {CombatSceneName}");
        SceneManager.LoadScene(CombatSceneName);
    }

    public void OnBattleResult(bool won)
    {
        if (!won)
        {
            Debug.Log("Battle Lost. Returning to Lab.");
            EndRun(false);
            return;
        }

        Debug.Log($"Battle Won! Encounter {currentEncounterIndex} complete.");
        AddRandomLoot(1);

        if (currentEncounterIndex == 1)
        {
            currentEncounterIndex = 2;
            LoadNextCombat();
        }
        else if (currentEncounterIndex == 2)
        {
            float roll = Random.value;
            if (roll < 0.2f)
            {
                currentEncounterIndex = 4;
                LoadTreasureRoom();
            }
            else
            {
                currentEncounterIndex = 3;
                LoadNextCombat();
            }
        }
        else if (currentEncounterIndex == 3)
        {
            currentEncounterIndex = 4;
            LoadTreasureRoom();
        }
    }

    private void LoadNextCombat()
    {
        Debug.Log($"Loading next combat: {CombatSceneName}");
        SceneManager.LoadScene(CombatSceneName);
    }

    private void LoadTreasureRoom()
    {
        Debug.Log("Treasure Reached! Ending run and returning to lab for rewards.");
        AddRandomLoot(3);
        EndRun(true);
    }

    private void AddRandomLoot(int count)
    {
        if (database == null || database.Items == null || database.Items.Length == 0) return;

        var ingredients = database.Items.Where(i => i.itemType == ItemType.Ingredient).ToList();
        if (ingredients.Count == 0) return;

        for (int i = 0; i < count; i++)
        {
            var randomItem = ingredients[Random.Range(0, ingredients.Count)];
            sessionLoot.Add(new Item(randomItem));
            Debug.Log($"Loot added: {randomItem.ItemName}");
        }
    }

    public void EndRun(bool won)
    {
        if (won)
        {
            pendingTreasureDisplay = true;

            foreach (var item in sessionLoot)
            {
                playerInventory.AddItem(item, 1, "");
            }

            foreach (var slot in carriedPotions)
            {
                if (slot.amount > 0)
                    playerInventory.AddItem(slot.item, slot.amount, slot.item.VariantKey);
            }
        }
        else
        {
            pendingTreasureDisplay = false;
            sessionLoot.Clear();
            carriedPotions.Clear();
        }

        isRunActive = false;
        playerInventory.Save();
        Debug.Log($"Run ended. Attempting to load Lab Scene: '{LabSceneName}'");

        if (Application.CanStreamedLevelBeLoaded(LabSceneName))
        {
            SceneManager.LoadScene(LabSceneName);
        }
        else
        {
            Debug.LogError($"CRITICAL ERROR: Scene '{LabSceneName}' cannot be loaded! Check Build Settings (File > Build Profiles).");
        }
    }
}
