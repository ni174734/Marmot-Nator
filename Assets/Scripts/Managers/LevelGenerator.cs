using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    [Header("Shared Spawn Pool")]
    public Transform[] spawnPoints;

    [Header("Food Prefabs")]
    public GameObject[] healthyFoodPrefabs;
    public GameObject[] junkFoodPrefabs;
    public GameObject[] boostFoodPrefabs;

    [Header("Enemy Prefabs")]
    public GameObject marmotPrefab;
    public GameObject dogPrefab;
    public GameObject pestPrefab;

    [Header("Food Spawn Weights")]
    [Range(0f, 1f)] public float healthyChance = 0.5f;
    [Range(0f, 1f)] public float junkChance = 0.35f;
    [Range(0f, 1f)] public float boostChance = 0.15f;

    private readonly List<GameObject> spawnedObjects = new List<GameObject>();

    public void GenerateLevel(int level)
    {
        Debug.Log("GenerateLevel called for level " + level);

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL in LevelGenerator");
            return;
        }

        if (playerSpawnPoint == null)
        {
            Debug.LogError("playerSpawnPoint is NULL");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("spawnPoints array is empty");
            return;
        }

        ClearLevel();
        EnsurePlayerExistsAtSpawn();

        List<int> shuffled = GetShuffledIndices(spawnPoints.Length);
        int index = 0;

        int foodCount = GameManager.Instance.GetCurrentQuota();
        Debug.Log("Food to spawn: " + foodCount);

        for (int i = 0; i < foodCount && index < shuffled.Count; i++, index++)
        {
            Transform spawn = spawnPoints[shuffled[index]];
            GameObject prefab = GetRandomFoodPrefab();

            if (spawn == null)
            {
                Debug.LogWarning("A spawn point in spawnPoints is NULL");
                continue;
            }

            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, spawn.position, Quaternion.identity);
                spawnedObjects.Add(obj);
            }
        }

        int marmots = Mathf.Min(1 + level / 2, 3);
        int dogs = Mathf.Min(level / 2, 3);
        int pests = Mathf.Min(level / 3, 2);

        for (int i = 0; i < marmots && index < shuffled.Count; i++, index++)
            SpawnEnemy(marmotPrefab, spawnPoints[shuffled[index]]);

        for (int i = 0; i < dogs && index < shuffled.Count; i++, index++)
            SpawnEnemy(dogPrefab, spawnPoints[shuffled[index]]);

        for (int i = 0; i < pests && index < shuffled.Count; i++, index++)
            SpawnEnemy(pestPrefab, spawnPoints[shuffled[index]]);
    }

    private void EnsurePlayerExistsAtSpawn()
    {
        Transform player = GameManager.Instance.player;

        if (player == null)
        {
            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");

            if (existingPlayer != null)
            {
                player = existingPlayer.transform;
                Debug.Log("Found existing player in scene");
            }
            else if (playerPrefab != null)
            {
                GameObject newPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
                player = newPlayer.transform;
                Debug.Log("Spawned new player from prefab");
            }
            else
            {
                Debug.LogError("No existing player found and playerPrefab is NULL");
                return;
            }
        }

        GameManager.Instance.player = player;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        player.position = playerSpawnPoint.position;
    }

    private GameObject GetRandomFoodPrefab()
    {
        float total = healthyChance + junkChance + boostChance;
        if (total <= 0f) return null;

        float roll = Random.Range(0f, total);

        if (roll < healthyChance)
            return GetRandomFromArray(healthyFoodPrefabs);

        roll -= healthyChance;
        if (roll < junkChance)
            return GetRandomFromArray(junkFoodPrefabs);

        return GetRandomFromArray(boostFoodPrefabs);
    }

    private GameObject GetRandomFromArray(GameObject[] array)
    {
        if (array == null || array.Length == 0) return null;
        return array[Random.Range(0, array.Length)];
    }

    private void SpawnEnemy(GameObject prefab, Transform spawn)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Enemy prefab is NULL");
            return;
        }

        if (spawn == null)
        {
            Debug.LogWarning("Enemy spawn point is NULL");
            return;
        }

        GameObject enemy = Instantiate(prefab, spawn.position, Quaternion.identity);
        spawnedObjects.Add(enemy);
    }

    private void ClearLevel()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] != null)
                Destroy(spawnedObjects[i]);
        }

        spawnedObjects.Clear();
    }

    private List<int> GetShuffledIndices(int count)
    {
        List<int> list = new List<int>();

        for (int i = 0; i < count; i++)
            list.Add(i);

        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }

        return list;
    }
}