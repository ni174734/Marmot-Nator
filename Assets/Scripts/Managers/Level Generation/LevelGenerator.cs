using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    [Header("Level Start")]
    public Transform levelStartPoint;

    [Header("Chunk Generation")]
	public GameObject[] chunkPrefabs;
	public int chunksToGenerate = 6;
	public Transform firstConnector;

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
	
	[Header("Spawn Zone Rules")]
	[Range(0f, 1f)] public float foodZoneChance = 0.7f;
	public int maxEnemiesPerLevel = 5;
	public int maxFoodPerLevel = 12;
	
	[Header("End Block")]
	public GameObject endBlockPrefab;
	
	public float speedMultiplier = 1f;

	private int spawnedFoodCount;
	private int spawnedEnemyCount;

    private readonly List<GameObject> spawnedChunks = new List<GameObject>();
    private readonly List<GameObject> spawnedActors = new List<GameObject>();

    private readonly List<SpawnZone> spawnZones = new List<SpawnZone>();

    public void GenerateLevel(int level)
    {
        ClearLevel();
        EnsurePlayerExistsAtSpawn();
		
		GenerateChunks();
		
        CollectZones();
        PopulateSpawnZones(level);
    }

	private void ApplyEnemySpeedMultiplier(GameObject enemy, float speedMultiplier)
	{
		if (enemy == null) return;

		EnemyVisionChase chase = enemy.GetComponent<EnemyVisionChase>();
		if (chase != null)
		{
			chase.patrolSpeed *= speedMultiplier;
			chase.chaseSpeed *= speedMultiplier;
			return;
		}

		EnemyPatrol patrol = enemy.GetComponent<EnemyPatrol>();
		if (patrol != null)
		{
			patrol.speed *= speedMultiplier;
		}
	}

    private void EnsurePlayerExistsAtSpawn()
    {
        if (playerSpawnPoint == null || GameManager.Instance == null) return;

        Transform player = GameManager.Instance.player;

        if (player == null)
        {
            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");

            if (existingPlayer != null)
            {
                player = existingPlayer.transform;
            }
            else if (playerPrefab != null)
            {
                GameObject newPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
                player = newPlayer.transform;
            }
        }

        if (player != null)
        {
            GameManager.Instance.player = player;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            player.position = playerSpawnPoint.position;
        }
    }

	private void GenerateChunks()
	{
		if (chunkPrefabs == null || chunkPrefabs.Length == 0)
		{
			Debug.LogError("No chunk prefabs assigned.");
			return;
		}

		if (firstConnector == null)
		{
			Debug.LogError("No firstConnector assigned.");
			return;
		}

		Transform currentConnector = firstConnector;
		LevelChunk.ChunkType previousType = LevelChunk.ChunkType.Ground;

		for (int i = 0; i < chunksToGenerate; i++)
		{
			GameObject prefab = GetValidChunk(previousType);

			LevelChunk prefabChunk = prefab.GetComponent<LevelChunk>();
			GameObject chunk = Instantiate(prefab);

			LevelChunk placedChunk = chunk.GetComponent<LevelChunk>();

			Vector3 offset = placedChunk.startConnector.position - chunk.transform.position;
			chunk.transform.position = currentConnector.position - offset;

			spawnedChunks.Add(chunk);

			currentConnector = placedChunk.endConnector;
			
			previousType = placedChunk.chunkType;
		}
		
		PlaceEndBlock(currentConnector);
	}
	
	private void PlaceEndBlock(Transform currentConnector)
	{
		if (endBlockPrefab == null)
		{
			Debug.LogWarning("No endBlockPrefab assigned.");
			return;
		}

		GameObject endBlock = Instantiate(endBlockPrefab);
		LevelChunk endChunk = endBlock.GetComponent<LevelChunk>();

		if (endChunk == null || endChunk.startConnector == null)
		{
			Debug.LogError("End block prefab needs LevelChunk and startConnector.");
			Destroy(endBlock);
			return;
		}

		Vector3 offset = endChunk.startConnector.position - endBlock.transform.position;
		endBlock.transform.position = currentConnector.position - offset;

		spawnedChunks.Add(endBlock);
	}
	
	GameObject GetValidChunk(LevelChunk.ChunkType previousType)
	{
		for (int i = 0; i < 20; i++)
        {
            GameObject candidate = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
            LevelChunk chunk = candidate.GetComponent<LevelChunk>();

            if (chunk == null) continue;

            if (IsValidTransition(previousType, chunk.chunkType))
                return candidate;
        }

        return GetFallbackChunk();
	}
	
	bool IsValidTransition(LevelChunk.ChunkType prev, LevelChunk.ChunkType next)
    {
        if (next == LevelChunk.ChunkType.StairsDown)
        {
            if (prev != LevelChunk.ChunkType.Ground &&
                prev != LevelChunk.ChunkType.UpperGround)
                return false;
        }

        return true;
    }
	
	GameObject GetFallbackChunk()
    {
        foreach (var prefab in chunkPrefabs)
        {
            LevelChunk c = prefab.GetComponent<LevelChunk>();
            if (c != null && c.chunkType == LevelChunk.ChunkType.Ground)
                return prefab;
        }

        return chunkPrefabs[0];
    }

    private void CollectZones()
    {
        spawnZones.Clear();

        for (int i = 0; i < spawnedChunks.Count; i++)
        {
            if (spawnedChunks[i] == null) continue;

            SpawnZone[] zones = spawnedChunks[i].GetComponentsInChildren<SpawnZone>();

            for (int j = 0; j < zones.Length; j++)
            {
                spawnZones.Add(zones[j]);
            }
        }
    }

    private void PopulateSpawnZones(int level)
	{
		spawnedFoodCount = 0;
		spawnedEnemyCount = 0;
		
		if (GameManager.Instance != null)
			speedMultiplier = GameManager.Instance.GetEnemySpeedMultiplier();

		int requiredFood = GameManager.Instance.GetCurrentQuota();
		int foodTarget = requiredFood + 8;
		int enemyTarget = Mathf.Min(maxEnemiesPerLevel + level / 2, 12);

		List<SpawnZone> zones = new List<SpawnZone>(spawnZones);
		ShuffleList(zones);

		for (int i = 0; i < zones.Count; i++)
		{
			SpawnZone zone = zones[i];

			int amount = Random.Range(zone.minSpawns, zone.maxSpawns + 1);

			bool spawnFood = Random.value <= foodZoneChance;

			// Force food if we still don't have enough for quota
			if (spawnedFoodCount < requiredFood)
				spawnFood = true;

			if (spawnFood)
			{
				for (int j = 0; j < amount && spawnedFoodCount < foodTarget; j++)
				{
					GameObject prefab = GetRandomFoodPrefab();
					if (prefab == null) continue;

					GameObject obj = Instantiate(prefab, zone.GetRandomPoint(), Quaternion.identity);
					spawnedActors.Add(obj);
					spawnedFoodCount++;
				}
			}
			else
			{
				for (int j = 0; j < amount && spawnedEnemyCount < enemyTarget; j++)
				{
					GameObject prefab = GetRandomEnemyPrefab(level);
					if (prefab == null) continue;
					
					

					GameObject enemy = Instantiate(prefab, zone.GetRandomPoint(), Quaternion.identity);
					ApplyEnemySpeedMultiplier(enemy, speedMultiplier);
					spawnedActors.Add(enemy);
					spawnedEnemyCount++;
				}
			}
		}
	}
	
    private void SpawnEnemy(GameObject prefab, SpawnZone zone)
    {	
		if (prefab == null || zone == null) return;

        Vector3 pos = zone.GetRandomPoint();
        GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
        spawnedActors.Add(enemy);
    }

	private GameObject GetRandomEnemyPrefab(int level)
	{
		float marmotWeight = 1f;
		float dogWeight = level >= 2 ? 0.8f : 0f;
		float pestWeight = level >= 3 ? 0.35f : 0f;

		float total = marmotWeight + dogWeight + pestWeight;
		float roll = Random.Range(0f, total);

		if (roll < marmotWeight)
			return marmotPrefab;

		roll -= marmotWeight;
		if (roll < dogWeight)
			return dogPrefab;

		return pestPrefab;
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

    private void ClearLevel()
    {
		if (GameManager.Instance != null && GameManager.Instance.player != null)
		{
			playerController pc = GameManager.Instance.player.GetComponent<playerController>();
			if (pc != null)
				pc.ForceClearInteractable();
		}
		
        for (int i = 0; i < spawnedChunks.Count; i++)
        {
            if (spawnedChunks[i] != null)
                Destroy(spawnedChunks[i]);
        }

        for (int i = 0; i < spawnedActors.Count; i++)
        {
            if (spawnedActors[i] != null)
                Destroy(spawnedActors[i]);
        }

        spawnedChunks.Clear();
        spawnedActors.Clear();
        //foodZones.Clear();
        //enemyZones.Clear();
    }

	public void ClearGeneratedLevel()
	{
		ClearLevel();
	}

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }
}