using UnityEngine;

public class RandomMonsterSpawn : MonoBehaviour
{
    public enum MonsterType { Wolf, Tree, Slime, Bat }

    [Header("References")]
    public Transform player;

    [Header("Monster Prefabs")]
    public GameObject wolfPrefab;
    public GameObject treePrefab;
    public GameObject slimePrefab;
    public GameObject batPrefab;

    public Combatant SpawnMonster(Vector3 spawnPosition)
    {
        if (player == null)
        {
            Debug.LogError("Player reference NOT assigned on RandomMonsterSpawn!");
            return null;
        }

        MonsterType type = (MonsterType)Random.Range(
            0,
            System.Enum.GetValues(typeof(MonsterType)).Length
        );

        GameObject prefabToSpawn = type switch
        {
            MonsterType.Wolf => wolfPrefab,
            MonsterType.Tree => treePrefab,
            MonsterType.Slime => slimePrefab,
            MonsterType.Bat => batPrefab,
            _ => null
        };

        if (prefabToSpawn == null)
        {
            Debug.LogError("No prefab assigned for " + type);
            return null;
        }

        Quaternion rotation = GetEnemyRotation(spawnPosition);

        GameObject enemyObj = Instantiate(
            prefabToSpawn,
            spawnPosition,
            rotation
        );

        Combatant combatant = enemyObj.GetComponentInChildren<Combatant>();

        if (combatant == null)
        {
            Debug.LogError("Spawned enemy has NO Combatant component!");
            return null;
        }

        Debug.Log("Spawned enemy: " + enemyObj.name);
        return combatant;
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