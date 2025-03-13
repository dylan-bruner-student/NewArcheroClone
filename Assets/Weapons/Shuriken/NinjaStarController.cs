using System.Collections.Generic;
using UnityEngine;

public class NinjaStarController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 15f;
    public float rotationSpeed = 720f; // Degrees per second
    public float attackRange = 5f;
    public int maxChainCount
    {
        get
        {
            return NinjaStarManager.Instance.maxChainCount;
        }
    }

    public float damage
    {
        get
        {
            return NinjaStarManager.Instance.damage;
        }
    }

    [Header("Damage")]
    public float damageMultiplierPerChain = 0.8f; // Each chain does less damage

    private GameObject currentTarget;
    private int currentChainCount = 0;
    private List<GameObject> hitTargets = new List<GameObject>();
    private bool isMoving = false;
    private Vector2 startPosition;
    private float currentDamage;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    void Awake()
    {
        // Setup Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Let us control rotation manually
            rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Smoother movement
        }

        // Add a collider if needed
        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = 0.25f; // Adjust as needed
        }

        startPosition = transform.position;
        currentDamage = damage;
    }

    void Start()
    {
        // Find the first target
        currentTarget = FindNearestEnemyInRange(rb.position);
        if (currentTarget != null)
        {
            isMoving = true;
        }
        else
        {
            // No valid targets, destroy the shuriken
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Debug information to help troubleshoot
        if (currentTarget == null)
        {
            Debug.Log("No target found for shuriken");
        }

        // Only move if we have a target and are in moving state
        if (isMoving && currentTarget != null)
        {
            // Calculate direction to target
            Vector2 targetPosition = currentTarget.transform.position;
            Vector2 direction = (targetPosition - rb.position).normalized;

            // Debug.Log("Moving toward target: " + direction + " with speed: " + moveSpeed);

            // Move toward target
            rb.velocity = direction * moveSpeed;

            // Rotate the shuriken
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            // Check if we've reached the target (as a backup to trigger detection)
            float distanceToTarget = Vector2.Distance(rb.position, targetPosition);
            if (distanceToTarget < 0.2f) // Small threshold for hit detection
            {
                HitCurrentTarget();
            }
        }
        else
        {
            // Stop velocity if we're not moving to prevent drift
            rb.velocity = Vector2.zero;

            // Our target was destroyed or we don't have one
            if (isMoving && currentTarget == null)
            {
                FindNextTarget();
            }
        }
    }



    private void HitCurrentTarget()
    {
        // Apply damage to the enemy
        if (currentTarget.TryGetComponent<EnemyController>(out var enemyController))
        {
            enemyController.Damage(gameObject, currentDamage);
        }

        // Add target to hit list to avoid hitting it again
        hitTargets.Add(currentTarget);
        currentChainCount++;

        // If we've reached max chains, destroy the shuriken
        if (currentChainCount >= maxChainCount)
        {
            Destroy(gameObject);
            return;
        }

        // Reduce damage for next hit
        currentDamage *= damageMultiplierPerChain;

        // Find next target immediately
        FindNextTarget();
    }

    private void FindNextTarget()
    {
        // Find the next target
        currentTarget = FindNextEnemyInRange(rb.position);

        if (currentTarget != null)
        {
            isMoving = true;
        }
        else
        {
            // No more valid targets, destroy the shuriken
            Destroy(gameObject);
        }
    }

    private GameObject FindNearestEnemyInRange(Vector2 position)
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

            // Use Vector2.Distance for 2D distance calculation
            float distance = Vector2.Distance(position, enemy.transform.position);
            if (distance < attackRange && distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private GameObject FindNextEnemyInRange(Vector2 position)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
            return null;

        GameObject closestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            // Skip enemies we've already hit
            if (hitTargets.Contains(enemy))
                continue;

            // Skip enemies that aren't targetable
            if (enemy.TryGetComponent<EnemyController>(out var controller))
            {
                if (!controller.Targetable)
                    continue;
            }

            // Use Vector2.Distance for 2D distance calculation
            float distance = Vector2.Distance(position, enemy.transform.position);
            if (distance < attackRange && distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    // Trigger-based hit detection as an alternative/supplement to distance checking
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentTarget != null && collision.gameObject == currentTarget)
        {
            HitCurrentTarget();
        }
    }

    // Add this method to launch the shuriken from a specific position
    public static GameObject LaunchShuriken(GameObject shurikenPrefab, Vector2 position)
    {
        if (shurikenPrefab == null)
            return null;

        GameObject shuriken = Instantiate(shurikenPrefab, position, Quaternion.identity);
        return shuriken;
    }

    // Visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
