using _Project._Scripts.Sound_and_Music;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Clickforcamera : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject craftingCamera;
    public GameObject LabUi;
    public GameObject Return;
    public GameObject Craft;
    public GameObject Materialing;

    [Header("Battle Prep")]
    [SerializeField] private PotionSelectionPanel potionSelectionPanel;
    [SerializeField] private InventoryObject labInventory;
    private void Start()
    {
		Materialing.SetActive(false);
        Craft.SetActive(false);
        Return.SetActive(false);
        mainCamera.SetActive(true);
        craftingCamera.SetActive(false);

        //-----FMOD Integration-----
        if (AudioManager.Instance != null && FMODEvents.instance != null)
        {
            AudioManager.Instance.PlayMusic(FMODEvents.instance.baseMusic);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Adding random potion for testing...");
            AddRandomPotion();
        }
    }

    private void AddRandomPotion()
    {
        if (labInventory == null || labInventory.database == null) return;

        // find a potion item in the database
        ItemScriptableObject potionItem = null;
        foreach (var kvp in labInventory.database.GetItemByStableId)
        {
            if (kvp.Value.itemType == ItemType.Potion)
            {
                potionItem = kvp.Value;
                break;
            }
        }

        if (potionItem != null)
        {
            // use a dummy variant key just for testing if registry isn't handy, 
            // or maybe the potionItem has a default. 
            // For now, let's just add it with a generic key.
            labInventory.AddItem(new Item(potionItem), 1, "");
            Debug.Log($"Added 1 {potionItem.ItemName} to inventory.");
        }
    }

    public void SwitchToCrafting()
    {
		Materialing.SetActive(true);
        Craft.SetActive(true);
        Return.SetActive(true);
        LabUi.SetActive(false);
        Debug.Log("Switching to crafting camera");
        mainCamera.SetActive(false);
        craftingCamera.SetActive(true);

        //-----FMOD Integration-----
        if (AudioManager.Instance != null && FMODEvents.instance != null)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        }
    }

    public void SwitchToMain()
    {
		Materialing.SetActive(false);
        Craft.SetActive(false);
        Return.SetActive(false);
        LabUi.SetActive(true);
        Debug.Log("Switching to main camera");
        craftingCamera.SetActive(false);
        mainCamera.SetActive(true);

        //-----FMOD Integration-----
        if (AudioManager.Instance != null && FMODEvents.instance != null)
        {
            AudioManager.Instance.PlayMusic(FMODEvents.instance.baseMusic);
            AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        }
    }

    public void SwitchToCombat()
    {
        //-----FMOD Integration-----
        if (AudioManager.Instance != null && FMODEvents.instance != null)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.instance.expeditionStart);
        }
        //-----FMOD Integration-----

        Debug.LogError("[Clickforcamera] SwitchToCombat called.");
        if (labInventory == null) Debug.LogError("[Clickforcamera] LabInventory reference is missing!");
        if (potionSelectionPanel == null) Debug.LogError("[Clickforcamera] PotionSelectionPanel reference is missing!");

        // Count how many potions are in the lab inventory
        int potionCount = 0;
        if (labInventory != null && labInventory.database != null)
        {
            Debug.LogError($"[Clickforcamera] Checking inventory. Items count: {labInventory.Container.Items.Count}");
            foreach (var slot in labInventory.Container.Items)
            {
                bool found = labInventory.database.GetItemByStableId.TryGetValue(slot.item.StableId, out var itemSO);
                if (found)
                {
                    Debug.LogError($"[Clickforcamera] Found item: {itemSO.ItemName}, Type: {itemSO.itemType}, Amount: {slot.amount}");
                    if (itemSO.itemType == ItemType.Potion && slot.amount > 0)
                    {
                        potionCount += slot.amount;
                    }
                }
                else
                {
                    Debug.LogError($"[Clickforcamera] Item with StableID {slot.item.StableId} not found in database.");
                }
            }
        }

        Debug.LogError($"[Clickforcamera] Final Potion Count: {potionCount}");

        if (potionCount == 0 || potionSelectionPanel == null)
        {
            Debug.LogError("[Clickforcamera] Skipping selection panel (Count=0 or Panel=null). Loading Battle.");
            // No potions — go straight to battle
            BattleInventoryData.ClearAll();
            SceneManager.LoadSceneAsync(1);
        }
        else
        {
            Debug.LogError("[Clickforcamera] Opening PotionSelectionPanel.");
            // Open the selection panel; its Go button will load the scene
            potionSelectionPanel.Open();
        }
    }

    
}
