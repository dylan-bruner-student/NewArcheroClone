using UnityEngine;

public class NinjaStarManager : MonoBehaviour
{
    [Header("Shuriken Settings")]
    [SerializeField] private GameObject shurikenPrefab;
    [SerializeField] private float spawnDelay = 1.0f;
    [SerializeField] private float attackRange = 25f;
    [SerializeField] private Transform spawnPoint; // Optional - if null, will use player position

    public int maxChainCount = 3;
    public float damage = 30f;

    [Header("References")]
    [SerializeField] private Transform playerTransform; // Reference to the player

    private float lastSpawnTime = 0f;

    public static NinjaStarManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // If player reference not set, try to find it
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("NinjaStarManager: Player not found. Make sure the player has 'Player' tag.");
            }
        }

        // Initialize spawn time
        lastSpawnTime = Time.time;
    }

    private void Update()
    {
        // Check if it's time to spawn a new shuriken
        if (Time.time - lastSpawnTime >= spawnDelay)
        {
            // Only spawn if there's a valid enemy in range
            GameObject nearestEnemy = FindNearestEnemyInRange(transform.position);
            if (nearestEnemy != null)
            {
                SpawnShuriken();
                lastSpawnTime = Time.time; // Reset timer
            }
        }
    }

    private void SpawnShuriken()
    {
        if (shurikenPrefab == null)
        {
            Debug.LogError("NinjaStarManager: Shuriken prefab not assigned!");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("NinjaStarManager: Cannot spawn shuriken - player not found!");
            return;
        }

        NinjaStarController.LaunchShuriken(shurikenPrefab, transform.position);
    }


    private GameObject FindNearestEnemyInRange(Vector3 position)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
            return null;

        GameObject nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            // Skip enemies that aren't targetable
            if (enemy.TryGetComponent<EnemyController>(out var controller))
            {
                if (!controller.Targetable)
                    continue;
            }

            float distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < attackRange && distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    // Visualize the attack range in the editor
    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, attackRange);
    }
}
