using System.Collections;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private EnemyHealth normalEnemyPrefab;
    [SerializeField] private EnemyHealth healerEnemyPrefab;
    [SerializeField] private EnemyHealth tankEnemyPrefab;
    [SerializeField] private EnemyHealth shieldEnemyPrefab;

    [SerializeField] private int normalPrewarmCount = 10;
    [SerializeField] private int healerPrewarmCount = 3;
    [SerializeField] private int tankPrewarmCount = 3;
    [SerializeField] private int shieldPrewarmCount = 3;

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

    [Header("Healer Enemy Settings")]
    [SerializeField] private int firstHealerWave = 2;
    [SerializeField] private float healerSpawnChance = 0.15f;

    [Header("Tank Enemy Settings")]
    [SerializeField] private int firstTankWave = 3;
    [SerializeField] private float tankSpawnChance = 0.10f;

    [Header("Shield Enemy Settings")]
    [SerializeField] private int firstShieldWave = 4;
    [SerializeField] private float shieldSpawnChance = 0.10f;

    public int currentWave = 0;

    private ObjectPool<EnemyHealth> normalPool;
    private ObjectPool<EnemyHealth> healerPool;
    private ObjectPool<EnemyHealth> tankPool;
    private ObjectPool<EnemyHealth> shieldPool;

    private int enemiesToSpawnThisWave;
    private int enemiesSpawnedThisWave;
    private int enemiesAliveThisWave;

    private float currentSpawnInterval;

    private void Start()
    {
        normalPool = new ObjectPool<EnemyHealth>(normalEnemyPrefab, transform, normalPrewarmCount);
        healerPool = new ObjectPool<EnemyHealth>(healerEnemyPrefab, transform, healerPrewarmCount);
        tankPool = new ObjectPool<EnemyHealth>(tankEnemyPrefab, transform, tankPrewarmCount);
        shieldPool = new ObjectPool<EnemyHealth>(shieldEnemyPrefab, transform, shieldPrewarmCount);

        if (waveText != null)
            waveText.text = "Wave: 0";

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
                if (GetTotalActiveEnemies() < maxActiveEnemies && spawnPoints.Length > 0)
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
        }
    }

    private void StartNextWave()
    {
        currentWave++;

        enemiesSpawnedThisWave = 0;
        enemiesAliveThisWave = 0;

        enemiesToSpawnThisWave = startingEnemiesPerWave + (currentWave - 1) * extraEnemiesPerWave;

        currentSpawnInterval = baseSpawnInterval - (currentWave - 1) * spawnDecreasePerWave;

        if (currentSpawnInterval < minSpawnInterval)
            currentSpawnInterval = minSpawnInterval;

        if (waveText != null)
            waveText.text = "Wave: " + currentWave;

        Debug.Log("Starting Wave " + currentWave + " with " + enemiesToSpawnThisWave + " enemies.");
        Debug.Log("Spawn interval for wave " + currentWave + " = " + currentSpawnInterval);
    }

    private int GetTotalActiveEnemies()
    {
        return normalPool.CountActive + healerPool.CountActive + tankPool.CountActive + shieldPool.CountActive;
    }

    private void SpawnEnemy()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        EnemyHealth enemy;

        bool canSpawnHealer = currentWave >= firstHealerWave;
        bool canSpawnTank = currentWave >= firstTankWave;
        bool canSpawnShield = currentWave >= firstShieldWave;

        float randomValue = Random.value;

        if (canSpawnTank && randomValue < tankSpawnChance)
        {
            enemy = tankPool.Get(point.position, point.rotation);
        }
        else if (canSpawnShield && randomValue < tankSpawnChance + shieldSpawnChance)
        {
            enemy = shieldPool.Get(point.position, point.rotation);
        }
        else if (canSpawnHealer && randomValue < tankSpawnChance + shieldSpawnChance + healerSpawnChance)
        {
            enemy = healerPool.Get(point.position, point.rotation);
        }
        else
        {
            enemy = normalPool.Get(point.position, point.rotation);
        }

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

        if (enemy.Type == EnemyHealth.EnemyType.Healer)
        {
            healerPool.Return(enemy);
        }
        else if (enemy.Type == EnemyHealth.EnemyType.Tank)
        {
            tankPool.Return(enemy);
        }
        else if (enemy.Type == EnemyHealth.EnemyType.Shield)
        {
            shieldPool.Return(enemy);
        }
        else
        {
            normalPool.Return(enemy);
        }

        Debug.Log("Enemy died. Remaining alive in wave: " + enemiesAliveThisWave);
    }
}