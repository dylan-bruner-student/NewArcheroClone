using UnityEngine;

public class RailGunManager : BaseWeaponManager
{
    public float ShotDelay = 3f;
    public float AttackDamage = 100f;
    public float AttackRange = 100f;

    private float LastShotTime = 0;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float rayVisualDuration = 0.1f; // How long the ray stays visible
    private float rayVisualEndTime;

    private void Start()
    {
        // If not assigned in inspector, try to get or add the component
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();

                // Set up default LineRenderer properties
                lineRenderer.startWidth = 0.05f;
                lineRenderer.endWidth = 0.05f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = Color.cyan;
                lineRenderer.endColor = Color.cyan;
            }
        }

        // Initially disable the line renderer
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        // Hide the line renderer if its duration has expired
        if (lineRenderer.enabled && Time.time > rayVisualEndTime)
        {
            lineRenderer.enabled = false;
        }

        if (Time.time < LastShotTime + ShotDelay) return;

        GameObject target = FindNearestEnemyInRange();
        if (target == null) return;

        LastShotTime = Time.time;

        Vector2 origin = transform.position;
        Vector2 targetPosition = target.transform.position;
        Vector2 shootDirection = (targetPosition - origin).normalized;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, shootDirection, AttackRange);
        Debug.Log($"RailGun hit {hits.Length}");

        // Calculate the end point of the ray
        Vector2 endPoint;
        if (hits.Length > 0)
        {
            // If there are hits, find the furthest one
            float maxDistance = 0f;
            foreach (RaycastHit2D hit in hits)
            {
                float distance = Vector2.Distance(origin, hit.point);
                maxDistance = Mathf.Max(maxDistance, distance);
            }
            endPoint = origin + shootDirection * maxDistance;
        }
        else
        {
            // If no hits, use the full attack range
            endPoint = origin + shootDirection * AttackRange;
        }

        // Visualize the ray
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPoint);
        rayVisualEndTime = Time.time + rayVisualDuration;

        float modifier = 1.0f;
        foreach (RaycastHit2D hit in hits)
        {
            var controller = hit.collider.gameObject.GetComponent<EnemyController>();
            if (controller == null)
                continue;

            controller.Damage(gameObject, AttackDamage * modifier);
            Debug.Log($"Hit enemy for {AttackDamage * modifier}");
            modifier *= 0.9f;
        }
    }


    private GameObject FindNearestEnemyInRange()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (player == null || enemies.Length == 0)
            return null;

        GameObject nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (enemy.GetComponent<EnemyController>()?.Targetable != true)
                continue;

            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (distance < AttackRange && distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}