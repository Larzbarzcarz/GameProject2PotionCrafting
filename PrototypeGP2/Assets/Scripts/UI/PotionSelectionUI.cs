using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class PotionSelectionUI : MonoBehaviour
{
    public InventoryObject playerInventory;
    public GameObject selectionPanel;
    public GameObject potionButtonPrefab;
    public Transform potionContainer;
    public TextMeshProUGUI selectionCountText;
    public Button startButton;

    private List<InventorySlot> selectedPotions = new List<InventorySlot>();
    private const int MAX_POTIONS = 5;

    private void Start()
    {
        selectionPanel.SetActive(false);
        startButton.onClick.AddListener(OnStartExpedition);
    }

    public void OpenSelection()
    {
        selectionPanel.SetActive(true);
        selectedPotions.Clear();
        RefreshUI();
    }

    public void RefreshUI()
    {
        // Clear children
        foreach (Transform child in potionContainer)
        {
            Destroy(child.gameObject);
        }

        // Filter potions from inventory
        var potionsInInventory = playerInventory.Container.Items
            .Where(slot =>
            {
                if (playerInventory.database.GetItemByStableId.TryGetValue(slot.item.StableId, out var itemSO))
                {
                    return itemSO.itemType == ItemType.Potion;
                }
                return false;
            }).ToList();

        foreach (var slot in potionsInInventory)
        {
            GameObject btnObj = Instantiate(potionButtonPrefab, potionContainer);
            var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"{slot.item.Name} ({slot.amount})";

            var btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => TogglePotion(slot));

            // Highlight if selected
            if (selectedPotions.Any(s => s.item.StableId == slot.item.StableId && s.item.VariantKey == slot.item.VariantKey))
            {
                btn.image.color = Color.green;
            }
        }

        selectionCountText.text = $"Selected: {selectedPotions.Sum(s => s.amount)} / {MAX_POTIONS}";
        startButton.interactable = true; // User requested to test connection even without potions
    }

    private void Update()
    {
        // Debug: F1 to add a random potion
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("DEBUG: F1 Key Pressed in PotionSelectionUI");

            if (playerInventory == null)
            {
                Debug.LogError("DEBUG: PlayerInventory is NULL!");
                return;
            }
            if (playerInventory.database == null)
            {
                Debug.LogError("DEBUG: PlayerInventory Database is NULL!");
                return;
            }

            var allPotions = playerInventory.database.Items
                .Where(i => i.itemType == ItemType.Potion)
                .ToList();

            Debug.Log($"DEBUG: Found {allPotions.Count} potions in the database.");

            if (allPotions.Count > 0)
            {
                var randomPotion = allPotions[Random.Range(0, allPotions.Count)];
                playerInventory.AddItem(new Item(randomPotion), 1, "");
                Debug.Log($"DEBUG: Successfully added {randomPotion.name} to inventory.");
                RefreshUI();
            }
            else
            {
                Debug.LogWarning("Debug: No potions found in database! Check ItemType config.");
            }
        }
    }

    private void TogglePotion(InventorySlot slot)
    {
        var existing = selectedPotions.FirstOrDefault(s => s.item.StableId == slot.item.StableId && s.item.VariantKey == slot.item.VariantKey);

        if (existing != null)
        {
            selectedPotions.Remove(existing);
        }
        else
        {
            if (selectedPotions.Sum(s => s.amount) < MAX_POTIONS)
            {
                // For simplicity, we take 1 unit of this potion. 
                // In a more complex UI, we'd have a counter.
                selectedPotions.Add(new InventorySlot(slot.item, 1));
            }
        }
        RefreshUI();
    }

    private void OnStartExpedition()
    {
        if (EncounterManager.Instance != null)
        {
            // Auto-fill if empty
            if (selectedPotions.Count == 0)
            {
                Debug.Log("Auto-selecting up to 5 random potions from inventory...");
                var potionsInInventory = playerInventory.Container.Items
                    .Where(slot =>
                    {
                        if (playerInventory.database.GetItemByStableId.TryGetValue(slot.item.StableId, out var itemSO))
                        {
                            return itemSO.itemType == ItemType.Potion;
                        }
                        return false;
                    }).ToList();

                // Shuffle or just take top 5
                int countToTake = Mathf.Min(5, potionsInInventory.Count);
                for (int i = 0; i < countToTake; i++)
                {
                    selectedPotions.Add(new InventorySlot(potionsInInventory[i].item, 1));
                }
                Debug.Log($"Auto-selected {selectedPotions.Count} potions.");
            }

            EncounterManager.Instance.StartRun(selectedPotions);
        }
    }
}
