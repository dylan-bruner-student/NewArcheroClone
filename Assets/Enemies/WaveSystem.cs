using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveSystem : MonoBehaviour
{
    public static WaveSystem instance;

    [SerializeField] private GameObject m_CactusMine;
    [SerializeField] private GameObject m_Fish;
    [SerializeField] private GameObject m_SquareMan;

    [System.Serializable]
    public class EnemyType
    {
        public GameObject prefab;
        public string name;
        public float difficultyRating; // How "hard" this enemy is (1-10)
        public int minWaveToAppear = 1; // Minimum wave number when this enemy can appear
    }

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        [System.Serializable]
        public class EnemyGroup
        {
            public GameObject enemyPrefab;
            public int count;
            public float spawnDelay = 0.5f;
        }
        public List<EnemyGroup> enemies = new List<EnemyGroup>();
        public float timeBeforeNextWave = 5f;
        public float waveDifficulty; // Total calculated difficulty
    }

    [Header("Enemy Definitions")]
    [SerializeField] private List<EnemyType> enemyTypes = new List<EnemyType>();

    [Header("Wave Settings")]
    [SerializeField] private int currentWaveNumber = 0;
    [SerializeField] private float baseDifficulty = 10f;
    [SerializeField] private float difficultyScalingFactor = 1.2f;
    [SerializeField] private float baseTimeBetweenWaves = 5f;
    [SerializeField] private float minTimeBetweenWaves = 2f;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private Transform player;
    [SerializeField] private int preGeneratedWaves = 2; // How many waves to generate ahead

    [Header("Spawn Settings")]
    [SerializeField] private float baseSpawnDelay = 0.5f;
    [SerializeField] private float minSpawnDelay = 0.1f;

    [Header("Events")]
    public UnityEvent onWaveStart;
    public UnityEvent onWaveComplete;
    public UnityEvent<int> onNewWaveGenerated;

    [Header("Debug")]
    [SerializeField] private int remainingEnemies = 0;
    [SerializeField] private bool isSpawning = false;
    [SerializeField] private float currentWaveDifficulty = 0f;

    private Queue<Wave> waveQueue = new Queue<Wave>();
    private List<GameObject> activeEnemies = new List<GameObject>();
    private Wave currentWave;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        InitializeEnemyTypes();
    }

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Pre-generate initial waves
        for (int i = 0; i < preGeneratedWaves; i++)
        {
            GenerateNextWave();
        }

        StartCoroutine(Main());
    }

    private void InitializeEnemyTypes()
    {
        // Only initialize if list is empty
        if (enemyTypes.Count == 0)
        {
            enemyTypes.Add(new EnemyType
            {
                prefab = m_CactusMine,
                name = "Cactus Mine",
                difficultyRating = 1,
                minWaveToAppear = 1
            });

            enemyTypes.Add(new EnemyType
            {
                prefab = m_Fish,
                name = "Fish",
                difficultyRating = 2,
                minWaveToAppear = 2
            });

            enemyTypes.Add(new EnemyType
            {
                prefab = m_SquareMan,
                name = "Box Man",
                difficultyRating = 4,
                minWaveToAppear = 3
            });

            // Add more enemy types as needed
        }
    }

    private void GenerateNextWave()
    {
        currentWaveNumber++;

        // Calculate wave difficulty (increases exponentially)
        float waveDifficulty = baseDifficulty * Mathf.Pow(difficultyScalingFactor, currentWaveNumber - 1);

        // Create new wave
        Wave newWave = new Wave
        {
            waveName = "Wave " + currentWaveNumber,
            timeBeforeNextWave = Mathf.Max(minTimeBetweenWaves,
                                          baseTimeBetweenWaves - (currentWaveNumber * 0.1f)),
            waveDifficulty = waveDifficulty
        };

        // Get available enemies for this wave
        List<EnemyType> availableEnemies = enemyTypes.FindAll(e => e.minWaveToAppear <= currentWaveNumber);

        if (availableEnemies.Count == 0)
        {
            Debug.LogError("No available enemies for wave " + currentWaveNumber);
            return;
        }

        // Distribute difficulty among enemies
        float remainingDifficulty = waveDifficulty;
        while (remainingDifficulty > 0 && availableEnemies.Count > 0)
        {
            // Select random enemy type weighted by how recently they became available
            List<EnemyType> weightedEnemies = new List<EnemyType>();
            foreach (var enemy in availableEnemies)
            {
                // Recently unlocked enemies appear more frequently
                int recencyBonus = Mathf.Max(1, currentWaveNumber - enemy.minWaveToAppear + 1);
                for (int i = 0; i < recencyBonus; i++)
                {
                    weightedEnemies.Add(enemy);
                }
            }

            EnemyType selectedType = weightedEnemies[Random.Range(0, weightedEnemies.Count)];

            // Calculate how many of this enemy to spawn based on difficulty
            int count = Mathf.Max(1, Mathf.FloorToInt(remainingDifficulty / selectedType.difficultyRating));

            // Cap count to prevent excessive enemies
            int maxEnemies = 5 + currentWaveNumber / 2;
            count = Mathf.Min(count, maxEnemies);

            if (count > 0)
            {
                // Calculate spawn delay (decreases with wave number but has a minimum)
                float spawnDelay = Mathf.Max(minSpawnDelay,
                                            baseSpawnDelay - (currentWaveNumber * 0.02f));

                Wave.EnemyGroup group = new Wave.EnemyGroup
                {
                    enemyPrefab = selectedType.prefab,
                    count = count,
                    spawnDelay = spawnDelay
                };

                newWave.enemies.Add(group);
                remainingDifficulty -= count * selectedType.difficultyRating;
            }

            // Avoid infinite loops - if we can't place more enemies, break
            if (count == 0 || remainingDifficulty < availableEnemies[0].difficultyRating)
            {
                break;
            }
        }

        // Ensure we have at least one enemy group
        if (newWave.enemies.Count == 0 && availableEnemies.Count > 0)
        {
            EnemyType easiestType = availableEnemies[0];
            foreach (var enemy in availableEnemies)
            {
                if (enemy.difficultyRating < easiestType.difficultyRating)
                    easiestType = enemy;
            }

            newWave.enemies.Add(new Wave.EnemyGroup
            {
                enemyPrefab = easiestType.prefab,
                count = Mathf.Max(1, currentWaveNumber),
                spawnDelay = baseSpawnDelay
            });
        }

        waveQueue.Enqueue(newWave);
        onNewWaveGenerated?.Invoke(currentWaveNumber);

        Debug.Log($"Generated Wave {currentWaveNumber} with difficulty {waveDifficulty}");
    }

    private IEnumerator Main()
    {
        yield return new WaitForSeconds(1); // Initial delay

        while (true) // Infinite waves
        {
            if (waveQueue.Count > 0)
            {
                currentWave = waveQueue.Dequeue();
                currentWaveDifficulty = currentWave.waveDifficulty;

                Debug.Log($"Starting {currentWave.waveName} (Difficulty: {currentWaveDifficulty:F1})");

                onWaveStart?.Invoke();
                yield return StartCoroutine(SpawnWave(currentWave));

                // Wait until all enemies are defeated
                yield return StartCoroutine(WaitForWaveCompletion());

                onWaveComplete?.Invoke();
                Debug.Log($"{currentWave.waveName} completed!");

                // Always generate next wave to stay ahead
                if (waveQueue.Count < preGeneratedWaves)
                {
                    GenerateNextWave();
                }

                yield return new WaitForSeconds(currentWave.timeBeforeNextWave);
            }
            else
            {
                // Emergency backup - should never happen
                Debug.LogWarning("Wave queue empty! Generating new wave...");
                GenerateNextWave();
                yield return new WaitForSeconds(1);
            }
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;

        foreach (var enemyGroup in wave.enemies)
        {
            for (int i = 0; i < enemyGroup.count; i++)
            {
                SpawnEnemy(enemyGroup.enemyPrefab);
                yield return new WaitForSeconds(enemyGroup.spawnDelay);
            }
        }

        isSpawning = false;
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null || player == null) return;

        // Random position around player
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + new Vector3(randomDirection.x, randomDirection.y, 0) * spawnRadius;

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        activeEnemies.Add(enemy);
        remainingEnemies++;

        // Subscribe to enemy death event
        if (enemy.TryGetComponent<EnemyController>(out var enemyComponent))
        {
            enemyComponent.OnDeath += HandleEnemyDeath;
        }
        else
        {
            Debug.LogWarning("Enemy prefab does not have Enemy component!");
        }
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            remainingEnemies--;
        }
    }

    private IEnumerator WaitForWaveCompletion()
    {
        while (isSpawning || remainingEnemies > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Public methods for external access
    public int GetCurrentWave()
    {
        return currentWaveNumber;
    }

    public float GetCurrentWaveDifficulty()
    {
        return currentWaveDifficulty;
    }

    public int GetRemainingEnemies()
    {
        return remainingEnemies;
    }
}

// Enemy script remains the same as before
public class Enemy : MonoBehaviour
{


}
