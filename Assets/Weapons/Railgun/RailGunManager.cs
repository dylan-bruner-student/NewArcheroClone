using UnityEngine;

public class RailGunManager : BaseWeaponManager
{
    public float ShotDelay = 3f;
    public float AttackDamage = 100f;
    public float AttackRange = 100f;

    private float LastShotTime = 0;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LineRenderer secondLineRenderer; // Second line renderer for dual shots
    [SerializeField] private float rayVisualDuration = 0.1f; // How long the ray stays visible
    [SerializeField] private bool DualRailing = false;
    private float rayVisualEndTime;

    public void ToggleDualRailing()
    {
        DualRailing = !DualRailing;
        Debug.Log($"Dual Railing: {(DualRailing ? "Enabled" : "Disabled")}");
    }

    private void Start()
    {
        // If not assigned in inspector, try to get or add the component
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
                lineRenderer = gameObject.AddComponent<LineRenderer>();

        }

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        // Create second line renderer for dual rail
        if (secondLineRenderer == null)
        {
            GameObject secondLineObj = new GameObject("SecondRailLine");
            secondLineObj.transform.parent = transform;
            secondLineObj.transform.localPosition = Vector3.zero;
            secondLineRenderer = secondLineObj.AddComponent<LineRenderer>();

            // Match settings with first line renderer
            secondLineRenderer.startWidth = lineRenderer.startWidth;
            secondLineRenderer.endWidth = lineRenderer.endWidth;
            secondLineRenderer.material = lineRenderer.material;
            secondLineRenderer.startColor = Color.red;
            secondLineRenderer.endColor = Color.red;
        }

        // Initially disable the line renderers
        lineRenderer.enabled = false;
        secondLineRenderer.enabled = false;
    }

    private void Update()
    {
        // Hide the line renderers if their duration has expired
        if (Time.time > rayVisualEndTime)
        {
            lineRenderer.enabled = false;
            secondLineRenderer.enabled = false;
        }

        if (Time.time < LastShotTime + ShotDelay) return;

        GameObject target = FindNearestEnemyInRange();
        if (target == null) return;

        LastShotTime = Time.time;

        // Fire the first shot
        FireRailShot(target, lineRenderer);

        // If dual railing is enabled, fire a second shot at another target or the same target
        if (DualRailing)
        {
            GameObject secondTarget = FindSecondTarget(target);
            if (secondTarget != null)
            {
                FireRailShot(secondTarget, secondLineRenderer);
            }
            else
            {
                // If no second target found, fire at the same target again
                FireRailShot(target, secondLineRenderer);
            }
        }
    }

    private void FireRailShot(GameObject target, LineRenderer railLine)
    {
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
        railLine.enabled = true;
        railLine.positionCount = 2;
        railLine.SetPosition(0, origin);
        railLine.SetPosition(1, endPoint);
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

    private GameObject FindSecondTarget(GameObject firstTarget)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length <= 1)
            return null;

        GameObject secondTarget = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (enemy == firstTarget || enemy.GetComponent<EnemyController>()?.Targetable != true)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < AttackRange && distance < minDistance)
            {
                minDistance = distance;
                secondTarget = enemy;
            }
        }

        return secondTarget;
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
