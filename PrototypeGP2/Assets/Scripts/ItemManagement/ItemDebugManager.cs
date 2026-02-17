using UnityEngine;

/// <summary>
/// F1 — get 3 random ingredients
/// F2 — craft something based on what you have in inventory, put it in stash
/// F3 — fill battle pocket with 5 potions from stash
/// F4 — use potion on monster
/// F5 — use potion on enemy
/// F6 — log state of inventory, stash and pocket
/// </summary>
public class ItemDebugManager : MonoBehaviour
{
    private BattleSystem battleSystem;

    private void Update()
    {
        // Ensure ItemManager exists
        if (ItemManager.Instance == null)
        {
            if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.F2) ||
                Input.GetKeyDown(KeyCode.F3) || Input.GetKeyDown(KeyCode.F4) ||
                Input.GetKeyDown(KeyCode.F5) || Input.GetKeyDown(KeyCode.F6))
            {
                Debug.LogError("[ItemDebug] ItemManager.Instance is null, check if ItemManager exists in scene");
            }
            return;
        }

        // F1 — Spawn random ingredients
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("═══ DEBUG F1: SPAWN INGREDIENTS ═══");
            ItemManager.Instance.AddRandomIngredients(3);
        }

        // F2 — Craft random potion
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("═══ DEBUG F2: CRAFT RANDOM POTION ═══");
            ItemManager.Instance.CraftRandomPotion();
        }

        // F3 — Fill pocket from stash
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("═══ DEBUG F3: FILL POCKET FROM STASH ═══");
            ItemManager.Instance.FillPocketFromStash();
        }

        // F4 — Use potion on MONSTER
        if (Input.GetKeyDown(KeyCode.F4))
        {
            Debug.Log("═══ DEBUG F4: USE POTION ON MONSTER ═══");
            var bs = GetBattleSystem();
            if (bs == null || bs.monster == null)
            {
                Debug.LogWarning("[ItemDebug] No BattleSystem or monster found in scene.");
                return;
            }
            ItemManager.Instance.UseRandomPotionOnTarget(bs.monster, bs.monster);
        }

        // F5 — Use potion on ENEMY
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("═══ DEBUG F5: USE POTION ON ENEMY ═══");
            var bs = GetBattleSystem();
            if (bs == null || bs.enemy == null)
            {
                Debug.LogWarning("[ItemDebug] No battlesystem. Are you in battle?");
                return;
            }
            ItemManager.Instance.UseRandomPotionOnTarget(bs.enemy, bs.monster);
        }

        // F6 — Log state
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Debug.Log("═══ DEBUG F6: LOG ITEM STATE ═══");
            ItemManager.Instance.LogState();
        }
    }

    private BattleSystem GetBattleSystem()
    {
        if (battleSystem == null)
            battleSystem = FindFirstObjectByType<BattleSystem>();
        return battleSystem;
    }
}
