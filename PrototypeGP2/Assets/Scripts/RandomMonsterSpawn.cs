using UnityEngine;

public class RandomMonsterSpawn : MonoBehaviour
{
    public enum MonsterType { Tree, Slime, Bat }

    [Header("References")]
    public Transform player;

    [Header("Monster Prefabs")]
    public GameObject treePrefab;
    public GameObject slimePrefab;
    public GameObject batPrefab;

    private System.Collections.Generic.List<MonsterType> usedTypes = new System.Collections.Generic.List<MonsterType>();

    public void ResetHistory()
    {
        usedTypes.Clear();
    }

    public Combatant SpawnUniqueMonster(Vector3 spawnPosition)
    {
        if (player == null) return null;

        var allTypes = System.Enum.GetValues(typeof(MonsterType));
        System.Collections.Generic.List<MonsterType> available = new System.Collections.Generic.List<MonsterType>();

        foreach (MonsterType t in allTypes)
        {
            if (!usedTypes.Contains(t)) available.Add(t);
        }

        if (available.Count == 0)
        {
            Debug.Log("All monster types seen! Resetting history.");
            usedTypes.Clear();
            foreach (MonsterType t in allTypes) available.Add(t);
        }

        MonsterType selectedType = available[Random.Range(0, available.Count)];
        usedTypes.Add(selectedType);

        return SpawnSpecificMonster(selectedType, spawnPosition);
    }

    private Combatant SpawnSpecificMonster(MonsterType type, Vector3 spawnPosition)
    {
        GameObject prefabToSpawn = type switch
        {
            MonsterType.Tree => treePrefab,
            MonsterType.Slime => slimePrefab,
            MonsterType.Bat => batPrefab,
            _ => null
        };

        if (prefabToSpawn == null) return null;

        Quaternion rotation = GetEnemyRotation(spawnPosition);
        GameObject enemyObj = Instantiate(prefabToSpawn, spawnPosition, rotation);
        
        Combatant combatant = enemyObj.GetComponentInChildren<Combatant>();
        if (combatant != null) Debug.Log($"Spawned unique enemy: {type}");
        
        return combatant;
    }

    public Combatant SpawnMonster(Vector3 spawnPosition)
    {
        return SpawnUniqueMonster(spawnPosition);
    }

    private Quaternion GetEnemyRotation(Vector3 spawnPosition)
    {
        Vector3 direction = player.position - spawnPosition;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return Quaternion.identity;

        return Quaternion.LookRotation(direction);
    }
}