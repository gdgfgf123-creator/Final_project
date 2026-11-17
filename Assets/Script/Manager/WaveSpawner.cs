using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public Enemy[] enemyPrefabs;

        [Min(1)] public int enemyCount = 5;
        [Min(0.01f)] public float spawnInterval = 0.5f;
    }

    [Header("Enemy Waves")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField, Min(0f)] private float timeBetweenWaves = 3f;

    [Header("Boss")]
    [SerializeField] private Boss bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;

    [Header("Stage 2")]
    [SerializeField] private Wave[] stageTwoWaves;
    [SerializeField] private Boss stageTwoBossPrefab;

    private readonly List<Enemy> aliveEnemies = new List<Enemy>();

    private IEnumerator Start()
    {
        if (bossPrefab == null || bossSpawnPoint == null)
        {
            Debug.LogError("WaveSpawner: ใส่ Boss Prefab และ Boss Spawn Point", this);
            yield break;
        }

        if (!ValidateWaves(waves))
            yield break;

        yield return RunWaves(waves);

        Boss stageOneBoss = SpawnBoss(bossPrefab);

        if (stageTwoWaves == null || stageTwoWaves.Length == 0)
            yield break;

        if (!ValidateWaves(stageTwoWaves))
            yield break;

        if (!TryGetComponent<LevelManager>(out var levelManager))
            levelManager = gameObject.AddComponent<LevelManager>();

        yield return levelManager.WaitForStageOneClear(stageOneBoss);
        yield return RunWaves(stageTwoWaves);

        if (stageTwoBossPrefab != null)
            SpawnBoss(stageTwoBossPrefab);
    }

    private IEnumerator RunWaves(Wave[] stageWaves)
    {
        if (stageWaves == null)
            yield break;

        for (int waveIndex = 0; waveIndex < stageWaves.Length; waveIndex++)
        {
            Wave wave = stageWaves[waveIndex];

            yield return new WaitForSeconds(timeBetweenWaves);
            Debug.Log("Wave " + (waveIndex + 1), this);

            for (int i = 0; i < wave.enemyCount; i++)
            {
                Enemy prefab = wave.enemyPrefabs[
                    Random.Range(0, wave.enemyPrefabs.Length)
                ];

                Transform point = spawnPoints[
                    Random.Range(0, spawnPoints.Length)
                ];

                Enemy enemy = Instantiate(prefab, point.position, point.rotation);
                aliveEnemies.Add(enemy);

                yield return new WaitForSeconds(wave.spawnInterval);
            }

            while (aliveEnemies.Count > 0)
            {
                aliveEnemies.RemoveAll(
                    enemy => enemy == null || enemy.CurrentHP <= 0
                );

                if (aliveEnemies.Count > 0)
                    yield return null;
            }
        }
    }

    private Boss SpawnBoss(Boss prefab)
    {
        Boss boss = Instantiate(
            prefab,
            bossSpawnPoint.position,
            bossSpawnPoint.rotation
        );

        BossHealthUI healthUI = boss.GetComponentInChildren<BossHealthUI>(true);

        if (healthUI != null)
            healthUI.Show(boss);

        return boss;
    }

    private bool ValidateWaves(Wave[] stageWaves)
    {
        if (stageWaves == null || stageWaves.Length == 0)
            return true;

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("WaveSpawner: ยังไม่ได้ใส่ Spawn Points", this);
            return false;
        }

        foreach (Transform point in spawnPoints)
        {
            if (point == null)
            {
                Debug.LogError("WaveSpawner: มี Spawn Point ที่ยังว่าง", this);
                return false;
            }
        }

        foreach (Wave wave in stageWaves)
        {
            if (wave == null ||
                wave.enemyPrefabs == null ||
                wave.enemyPrefabs.Length == 0)
            {
                Debug.LogError("WaveSpawner: ใส่ Enemy Prefabs ให้ครบทุก Wave", this);
                return false;
            }

            foreach (Enemy prefab in wave.enemyPrefabs)
            {
                if (prefab == null || prefab is Boss)
                {
                    Debug.LogError(
                        "WaveSpawner: Enemy Prefabs ต้องเป็นศัตรูทั่วไปและห้ามเว้นว่าง",
                        this
                    );
                    return false;
                }
            }
        }

        return true;
    }
}