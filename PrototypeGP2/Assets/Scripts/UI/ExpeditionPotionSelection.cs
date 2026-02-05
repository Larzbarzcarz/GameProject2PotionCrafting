using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// 
public class ExpeditionPotionSelection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryObject potionInventory;
    [SerializeField] private int combatSceneIndex = 1;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private Transform potionListContainer;
    [SerializeField] private GameObject potionSlotPrefab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI selectedCountText;
    
    [Header("Settings")]
    [SerializeField] private int maxPotions = 5;
    
    private List<Item> availablePotions = new List<Item>();
    private List<Item> selectedPotions = new List<Item>();
    private List<PotionSlotUI> potionSlots = new List<PotionSlotUI>();
    
    private void Start()
    {
        if (selectionPanel != null)
            selectionPanel.SetActive(false);
            
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClicked);
            
        UpdateSelectedCountUI();
    }
    
    public void OpenSelection()
    {
        Debug.Log("[ExpeditionPotionSelection] Opening potion selection...");
        
        selectedPotions.Clear();
        ClearPotionSlots();
        
        LoadAvailablePotions();
        
        CreatePotionSlots();
        
        if (selectionPanel != null)
            selectionPanel.SetActive(true);
            
        UpdateSelectedCountUI();
    }
    
    public void CloseSelection()
    {
        if (selectionPanel != null)
            selectionPanel.SetActive(false);
            
        selectedPotions.Clear();
    }
    
    private void LoadAvailablePotions()
    {
        availablePotions.Clear();
        
        if (potionInventory == null || potionInventory.Container == null)
        {
            Debug.LogWarning("[ExpeditionPotionSelection] No potion inventory assigned!");
            return;
        }
        
        foreach (var slot in potionInventory.Container.Items)
        {
            if (slot.item != null && slot.amount > 0)
            {
                for (int i = 0; i < slot.amount; i++)
                {
                    availablePotions.Add(slot.item);
                }
            }
        }
        
        Debug.Log($"[ExpeditionPotionSelection] Found {availablePotions.Count} potions available");
    }
    
    private void CreatePotionSlots()
    {
        if (potionSlotPrefab == null || potionListContainer == null)
        {
            Debug.LogWarning("[ExpeditionPotionSelection] Missing prefab or container reference!");
            return;
        }
        
        foreach (var potion in availablePotions)
        {
            GameObject slotObj = Instantiate(potionSlotPrefab, potionListContainer);
            PotionSlotUI slotUI = slotObj.GetComponent<PotionSlotUI>();
            
            if (slotUI != null)
            {
                slotUI.Setup(potion, OnPotionSlotClicked);
                potionSlots.Add(slotUI);
            }
            else
            {
                var text = slotObj.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                    text.text = potion.Name ?? "Unknown Potion";
                    
                var button = slotObj.GetComponent<Button>();
                if (button != null)
                {
                    int index = potionSlots.Count;
                    button.onClick.AddListener(() => TogglePotionSelection(potion));
                }
            }
        }
    }
    
    private void ClearPotionSlots()
    {
        foreach (var slot in potionSlots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        potionSlots.Clear();
        
        if (potionListContainer != null)
        {
            foreach (Transform child in potionListContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    private void OnPotionSlotClicked(PotionSlotUI slot)
    {
        TogglePotionSelection(slot.Potion);
        slot.SetSelected(selectedPotions.Contains(slot.Potion));
    }
    
    private void TogglePotionSelection(Item potion)
    {
        if (selectedPotions.Contains(potion))
        {
            selectedPotions.Remove(potion);
            Debug.Log($"[ExpeditionPotionSelection] Deselected: {potion.Name}");
        }
        else if (selectedPotions.Count < maxPotions)
        {
            selectedPotions.Add(potion);
            Debug.Log($"[ExpeditionPotionSelection] Selected: {potion.Name}");
        }
        else
        {
            Debug.Log($"[ExpeditionPotionSelection] Cannot select more than {maxPotions} potions!");
        }
        
        UpdateSelectedCountUI();
    }
    
    private void UpdateSelectedCountUI()
    {
        if (selectedCountText != null)
            selectedCountText.text = $"Selected: {selectedPotions.Count}/{maxPotions}";
            
        if (confirmButton != null)
            confirmButton.interactable = true; 
    }
    
    private void OnConfirmClicked()
    {
        Debug.Log($"[ExpeditionPotionSelection] Confirming with {selectedPotions.Count} potions");
        
        ExpeditionData.StartExpedition(selectedPotions);
        
        foreach (var potion in selectedPotions)
        {
            if (potionInventory != null)
            {
                potionInventory.RemoveItem(potion.StableId, 1);
            }
        }
        
        if (potionInventory != null)
            potionInventory.Save();
        
        SceneManager.LoadScene(combatSceneIndex);
    }
}
