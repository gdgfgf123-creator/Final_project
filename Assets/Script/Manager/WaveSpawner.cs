using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public Enemy[] enemyPrefabs;

        [Min(1)]
        public int enemyCount = 5;

        [Min(0.01f)]
        public float spawnInterval = 0.5f;
    }

    [Header("Enemy Waves")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField, Min(0f)] private float timeBetweenWaves = 3f;

    [Header("Boss")]
    [SerializeField] private Boss bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private BossHealthUI bossHealthUI;

    private readonly List<Enemy> aliveEnemies = new List<Enemy>();

    private bool bossSpawned;

    private IEnumerator Start()
    {
        if (!ValidateSetup())
            yield break;

        for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
        {
            Wave wave = waves[waveIndex];

            yield return new WaitForSeconds(timeBetweenWaves);

            Debug.Log("Wave " + (waveIndex + 1));

            for (int i = 0; i < wave.enemyCount; i++)
            {
                if (bossSpawned)
                    yield break;

                Enemy prefab = wave.enemyPrefabs[
                    Random.Range(0, wave.enemyPrefabs.Length)
                ];

                Transform point = spawnPoints[
                    Random.Range(0, spawnPoints.Length)
                ];

                Enemy enemy = Instantiate(
                    prefab,
                    point.position,
                    point.rotation
                );

                aliveEnemies.Add(enemy);

                yield return new WaitForSeconds(wave.spawnInterval);
            }

            // รอให้ศัตรูที่สร้างใน Wave นี้ตายหมด
            while (aliveEnemies.Count > 0)
            {
                aliveEnemies.RemoveAll(
                    enemy => enemy == null || enemy.CurrentHP <= 0
                );

                yield return null;
            }
        }

        SpawnBoss();
    }

    private void SpawnBoss()
    {
        if (bossSpawned)
            return;

        bossSpawned = true;

        Boss boss = Instantiate(
            bossPrefab,
            bossSpawnPoint.position,
            bossSpawnPoint.rotation
        );

        if (bossHealthUI != null)
            bossHealthUI.Show(boss);

        Debug.Log("Boss spawned");
    }

    private bool ValidateSetup()
    {
        if (bossPrefab == null || bossSpawnPoint == null)
        {
            Debug.LogError("WaveSpawner: ใส่ Boss Prefab และ Boss Spawn Point", this);
            return false;
        }

        if (waves == null || waves.Length == 0)
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

        foreach (Wave wave in waves)
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