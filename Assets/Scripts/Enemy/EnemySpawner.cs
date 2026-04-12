using System.Collections;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private EnemyHealth enemyPrefab;
    [SerializeField] private int prewarmCount = 5;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave UI")]
    [SerializeField] private TMP_Text waveText;

    [Header("Wave Settings")]
    [SerializeField] private float baseSpawnInterval = 0.8f;
    [SerializeField] private float spawnDecreasePerWave = 0.1f;
    [SerializeField] private float minSpawnInterval = 0.3f;
    [SerializeField] private float timeBetweenWaves = 2f;
    [SerializeField] private int startingEnemiesPerWave = 6;
    [SerializeField] private int extraEnemiesPerWave = 3;
    [SerializeField] private int maxActiveEnemies = 15;

    public int currentWave = 0;

    private ObjectPool<EnemyHealth> pool;

    private int enemiesToSpawnThisWave;
    private int enemiesSpawnedThisWave;
    private int enemiesAliveThisWave;

    private float currentSpawnInterval;
    private bool waveInProgress = false;

    private void Start()
    {
        pool = new ObjectPool<EnemyHealth>(enemyPrefab, transform, prewarmCount);

        if (waveText != null)
        {
            waveText.text = "Wave: 0";
        }

        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            StartNextWave();

            while (enemiesSpawnedThisWave < enemiesToSpawnThisWave)
            {
                if (pool.CountActive < maxActiveEnemies && spawnPoints.Length > 0)
                {
                    SpawnEnemy();
                    enemiesSpawnedThisWave++;
                    enemiesAliveThisWave++;
                }

                yield return new WaitForSeconds(currentSpawnInterval);
            }

            while (enemiesAliveThisWave > 0)
            {
                yield return null;
            }

            waveInProgress = false;
        }
    }

    private void StartNextWave()
    {
        currentWave++;
        waveInProgress = true;

        enemiesSpawnedThisWave = 0;
        enemiesAliveThisWave = 0;

        enemiesToSpawnThisWave = startingEnemiesPerWave + (currentWave - 1) * extraEnemiesPerWave;

        currentSpawnInterval = baseSpawnInterval - (currentWave - 1) * spawnDecreasePerWave;

        if (currentSpawnInterval < minSpawnInterval)
            currentSpawnInterval = minSpawnInterval;

        if (waveText != null)
        {
            waveText.text = "Wave: " + currentWave;
        }

        Debug.Log("Starting Wave " + currentWave + " with " + enemiesToSpawnThisWave + " enemies.");
        Debug.Log("Spawn interval for wave " + currentWave + " = " + currentSpawnInterval);
    }

    private void SpawnEnemy()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        EnemyHealth enemy = pool.Get(point.position, point.rotation);

        enemy.SetWaveDifficulty(currentWave);

        EnemyDamage enemyDamage = enemy.GetComponentInChildren<EnemyDamage>();
        if (enemyDamage != null)
        {
            enemyDamage.SetWaveDamage(currentWave);
        }

        enemy.OnDied -= HandleEnemyDied;
        enemy.OnDied += HandleEnemyDied;
    }

    private void HandleEnemyDied(EnemyHealth enemy)
    {
        enemy.OnDied -= HandleEnemyDied;

        enemiesAliveThisWave--;

        pool.Return(enemy);

        Debug.Log("Enemy died. Remaining alive in wave: " + enemiesAliveThisWave);
    }
}