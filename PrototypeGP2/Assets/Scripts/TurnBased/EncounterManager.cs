using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EncounterManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int labSceneIndex = 0;
    
    [Header("References")]
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject intermissionPanel;
    [SerializeField] private GameObject defeatPanel;

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

        if (MutationManager.Instance != null)
        {
            MutationManager.Instance.OnBattleStart();
        }
        
        if (battleSystem != null)
        {
            battleSystem.ResetForNewEncounter();
            battleSystem.BeginBattle();
        }
    }

    private void HandleBattleWon()
    {
        Debug.Log("[EncounterManager] Battle Won!");

        if (MutationManager.Instance != null)
        {
            MutationManager.Instance.OnBattleEnd(true, battleSystem);
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
            if (victoryPanel != null) victoryPanel.SetActive(true);
            ExpeditionData.EndExpedition();
        }
    }

    private void HandleBattleLost()
    {
        Debug.Log("[EncounterManager] Battle Lost.");

        if (MutationManager.Instance != null)
        {
            MutationManager.Instance.OnBattleEnd(false, battleSystem);
        }

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
