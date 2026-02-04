using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro support

public class RadialMenu : MonoBehaviour
{
    [Header("Main Buttons")]
    public Button attackButton;
    public Button defendButton;
    public Button inventoryButton;

    [Header("Attack Sub Buttons")]
    public Button heavyAttackButton;
    public Button lightAttackButton;

    [Header("UI Text (TMP)")]
    public TMP_Text actionText; 

    [Header("Player Turn")]
    [SerializeField] private bool _playersTurn = true; // backing field for property

    public bool playersTurn
    {
        get => _playersTurn;
        set
        {
            _playersTurn = value;
            RefreshMenu(); // auto-refresh menu whenever turn changes
        }
    }

    [Header("Debug System")]
    public bool showHeavyLight = false;   // Editor toggle
    public string currentAction = "";

    private bool runtimeStarted = false; // to know if we are in play mode

    private void Awake()
    {
        // Setup button listeners
        attackButton.onClick.AddListener(OnAttackButton);
        heavyAttackButton.onClick.AddListener(() => OnAttackChoice("Heavy Attack"));
        lightAttackButton.onClick.AddListener(() => OnAttackChoice("Light Attack"));
        defendButton.onClick.AddListener(OnDefend);
        inventoryButton.onClick.AddListener(OnInventory);
    }

    private void Start()
    {
        runtimeStarted = true; // mark runtime
        HideAttackSubButtons(); // hide sub-buttons at runtime
        RefreshMenu();
    }

    private void Update()
    {
        // Enable/disable buttons based on turn
        attackButton.interactable = playersTurn;
        defendButton.interactable = playersTurn;
        inventoryButton.interactable = playersTurn;

        // Example of turn-based logic placeholder
        if (playersTurn)
        {
            // PUT TURN LOGIC HERE
            // Example: enable player input, highlight UI, etc.
        }
        else
        {
            // PUT TURN LOGIC HERE
            // Example: AI takes turn, disable player input, etc.
        }
    }

    private void OnAttackButton()
    {
        if (!playersTurn) return;

        actionText.text = "Choose Attack";
        heavyAttackButton.gameObject.SetActive(true);
        lightAttackButton.gameObject.SetActive(true);
        showHeavyLight = true;
    }

    private void OnAttackChoice(string choice)
    {
        Debug.Log("Attack chosen: " + choice);
        actionText.text = choice;
        HideAttackSubButtons();
        showHeavyLight = false;
        currentAction = choice;

        // PUT ATTACK LOGIC HERE
        // Example: deal damage to enemy, trigger animation, etc.
    }

    private void OnDefend()
    {
        if (!playersTurn) return;

        Debug.Log("Chosen Defend");
        actionText.text = "Defend";
        HideAttackSubButtons();
        showHeavyLight = false;
        currentAction = "Defend";

        // PUT DEFEND LOGIC HERE
        // Example: increase player defense, play animation, etc.
    }

    private void OnInventory()
    {
        if (!playersTurn) return;

        actionText.text = "Inventory";
        HideAttackSubButtons();
        showHeavyLight = false;
        currentAction = "Inventory";

        Debug.Log("OPENED INVENTORY");
        // PUT OPEN INVENTORY LOGIC HERE
        // Example: open inventory UI, allow item selection, etc.
    }

    private void OnInventoryClose()
    {
        actionText.text = "Closed Inventory";

        // PUT CLOSE INVENTORY LOGIC HERE
    }

    private void HideAttackSubButtons()
    {
        if (!runtimeStarted) return; // Only hide at runtime
        heavyAttackButton.gameObject.SetActive(false);
        lightAttackButton.gameObject.SetActive(false);
    }

    private void UpdateMenuVisuals()
    {
        // Update colors based on turn to make stuff transparent when not ur turn
        Color menuColor = playersTurn ? Color.white : new Color(0.7f, 0.7f, 0.7f, 0.5f);
        attackButton.image.color = menuColor;
        defendButton.image.color = menuColor;
        inventoryButton.image.color = menuColor;

        // Update dialogue text
        if (!playersTurn)
            actionText.text = "Enemy's Turn";
        else if (currentAction == "" && !showHeavyLight)
            actionText.text = "Your Turn";
    }

    public void RefreshMenu()
    {
        // Refresh everything manually
        HideAttackSubButtons();
        UpdateMenuVisuals();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Editor debug toggle for previewing buttons without goofyness
        if (heavyAttackButton != null && lightAttackButton != null)
        {
            heavyAttackButton.gameObject.SetActive(showHeavyLight);
            lightAttackButton.gameObject.SetActive(showHeavyLight);
        }

        UpdateMenuVisuals();
    }
#endif
}