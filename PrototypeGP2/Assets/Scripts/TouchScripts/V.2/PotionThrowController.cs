using UnityEngine;

public class PotionThrowController : MonoBehaviour
{
    [Header("References")]
    public Camera cam;

    [Header("Targeting")]
    public LayerMask enemyMask;

    [Header("Optional visuals")]
    public GameObject potionGhostPrefab;
    public float ghostDistanceFromCamera;

    private PotionBaseSO selectedPotion;
    private bool throwMode;

    private GameObject ghost;
    public InventoryObject inventory;

    private void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!cam) Debug.LogError("PotionThrowController: No camera assigned.");
    }

    public void SelectPotion(PotionBaseSO potion)
    {
        if (potion == null)
        {
            Debug.LogWarning($"Potion '{potion.name}' has no recipe assigned (PotionRecipeSO).");
        }

        selectedPotion = potion;
        throwMode = true;

        SpawnGhostIfNeeded();
        Debug.Log($"Selected porion: {potion.name}");
    }

    public bool HasPotionSelected()
    {
        return throwMode && selectedPotion != null;
    }

    public void UpdateAim(Vector2 screenPos)
    {
        if (!HasPotionSelected() || cam == null) return;

        if (ghost != null)
        {
            Vector3 sp = new Vector3(screenPos.x, screenPos.y, ghostDistanceFromCamera);
            ghost.transform.position = cam.ScreenToWorldPoint(sp);
        }
    }

    public void TryThrow(Vector2 screenPos)
    {
        if (!HasPotionSelected() || cam == null) return;

        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, enemyMask))
        {
            var target = hit.collider.GetComponentInParent<Combatant>();
            if (target != null)
            {
                Debug.Log($"Potion hit target: {target.name}");

                if (selectedPotion.recipe != null && selectedPotion.recipe.entries.Count > 0)
                {
                    var entry = selectedPotion.recipe.entries[0];
                    // create temporary PotionData from the SO recipe to use with new system
                    var tempPotionData = new PotionData(entry.mainKey, entry.baseKey);
                    tempPotionData.playerName = selectedPotion.ItemName;

                    // apply effect (casterMonster is null here as we don't have reference, SwapHp might fail)
                    PotionEffectApplier.Apply(tempPotionData, target, null);
                }
                else
                {
                    Debug.LogWarning("[THROW] Selected potion has no recipe entries!");
                }

                if (inventory != null)
                {
                    bool ok = inventory.RemoveItem(selectedPotion.StableId, 1);
                    Debug.Log(ok ? "[THROW] Consumed 1 potion." : "[THROW] Could not remove potion from inventory!");
                }
            }
            else
            {
                Debug.Log("Hit enemy layer object but no Combatant found in parent");
            }
        }
        else
        {
            Debug.Log("Throw missed (raycast hit nothing on enemyMask).");
        }

        CancelThrowMode();
    }

    public void CancelThrowMode()
    {
        throwMode = false;
        selectedPotion = null;

        if (ghost != null)
        {
            Destroy(ghost);
            ghost = null;
        }
    }

    private void SpawnGhostIfNeeded()
    {
        if (potionGhostPrefab == null) return;

        if (ghost != null)
            Destroy(ghost);

        ghost = Instantiate(potionGhostPrefab);
    }
}
