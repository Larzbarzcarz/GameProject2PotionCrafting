using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// 
public class EncounterManager : MonoBehaviour
{
    [Header("Scene Indices")]
    [SerializeField] private int labSceneIndex = 0;
    
    [Header("Rewards")]
    [SerializeField] private int finalVictoryIngredientCount = 10;
    
    [Header("References")]
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform enemySpawnPoint;
    
    [Header("UI References")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject intermissionPanel;
    
    private Enemy currentEnemy;
    
    public static EncounterManager Instance { get; private set; }
    
    public event Action OnEncounterStarted;
    public event Action OnExpeditionComplete;
    public event Action OnExpeditionFailed;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        Debug.Log($"[EncounterManager] Starting encounter {ExpeditionData.CurrentEncounter}/{ExpeditionData.TotalEncounters}");
        
        if (battleSystem != null)
        {
            battleSystem.OnBattleWon += HandleBattleWon;
            battleSystem.OnBattleLost += HandleBattleLost;
        }
        else
        {
            Debug.LogError("[EncounterManager] BattleSystem reference not set!");
        }
        
        HideAllPanels();
    }
    
    private void OnDestroy()
    {
        if (battleSystem != null)
        {
            battleSystem.OnBattleWon -= HandleBattleWon;
            battleSystem.OnBattleLost -= HandleBattleLost;
        }
    }
    
    private void HandleBattleWon()
    {
        Debug.Log($"[EncounterManager] Battle won! Encounter {ExpeditionData.CurrentEncounter}/{ExpeditionData.TotalEncounters}");
        
        if (ExpeditionData.AdvanceEncounter())
        {
            ShowIntermissionPanel();
        }
        else
        {
            HandleExpeditionVictory();
        }
    }
    
    private void HandleBattleLost()
    {
        Debug.Log("[EncounterManager] Battle lost!");
        ExpeditionData.FailExpedition();
        ShowDefeatPanel();
        OnExpeditionFailed?.Invoke();
    }
    
    private void HandleExpeditionVictory()
    {
        Debug.Log($"[EncounterManager] Expedition complete! Awarding {finalVictoryIngredientCount} ingredients.");
        ExpeditionData.CompleteExpedition();
        
        Debug.Log($"[EncounterManager] Would add {finalVictoryIngredientCount} random ingredients to inventory.");
        
        ShowVictoryPanel();
        OnExpeditionComplete?.Invoke();
    }
    
    public void StartNextEncounter()
    {
        Debug.Log($"[EncounterManager] Starting encounter {ExpeditionData.CurrentEncounter}");
        HideAllPanels();
        
        if (battleSystem != null && battleSystem.enemy != null)
        {
            battleSystem.ResetForNewEncounter();
        }
        
        OnEncounterStarted?.Invoke();
    }
    
    public void ReturnToLab()
    {
        Debug.Log("[EncounterManager] Returning to lab...");
        ExpeditionData.Reset();
        SceneManager.LoadScene(labSceneIndex);
    }
    
    private void ShowVictoryPanel()
    {
        HideAllPanels();
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }
    
    private void ShowDefeatPanel()
    {
        HideAllPanels();
        if (defeatPanel != null) defeatPanel.SetActive(true);
    }
    
    private void ShowIntermissionPanel()
    {
        HideAllPanels();
        if (intermissionPanel != null) intermissionPanel.SetActive(true);
    }
    
    private void HideAllPanels()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (intermissionPanel != null) intermissionPanel.SetActive(false);
    }
}
