using System.Collections;
using UnityEngine;

public class EyeSuckerController : EnemyController
{
    public Sprite m_OpenEye;

    private Animator animator;
    private float Health = 75f;

    private bool LockedOn = false;
    private float TimeTillLock = 0;
    private bool HasTeleportedIntoRange = false; // Track if already teleported

    // Config
    private float n_TimeToLock = 5f;
    private float n_MaxLockRange = 25f;
    [SerializeField] private float DamagePerSecond = 20f;
    [SerializeField] private float BaseAnimationSpeed = 1.0f;
    [SerializeField] private float MaxAnimationSpeed = 3.0f;

    // Teleport config
    [SerializeField] private float TeleportProbability = 0.01f; // Chance per frame to teleport
    [SerializeField] private float MinTeleportRange = 8f;       // Minimum distance from player
    [SerializeField] private float MaxTeleportRange = 20f;      // Maximum distance from player

    private Coroutine damageCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();
        // Set initial animation speed
        animator.speed = BaseAnimationSpeed;
    }

    void Update()
    {
        var player = PlayerController.Instance.gameObject;
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Check if player is in range
        if (distanceToPlayer <= n_MaxLockRange)
        {
            if (!LockedOn)
            {
                // Only consider teleporting if we haven't teleported yet
                if (!HasTeleportedIntoRange && Random.value < TeleportProbability)
                {
                    TeleportRandomlyAroundPlayer(player.transform.position);
                    HasTeleportedIntoRange = true; // Mark as teleported
                }

                // Increment lock timer
                TimeTillLock += Time.deltaTime;

                // Calculate lock progress (0 to 1)
                float lockProgress = Mathf.Clamp01(TimeTillLock / n_TimeToLock);

                // Increase animation speed linearly based on lock progress
                animator.speed = Mathf.Lerp(BaseAnimationSpeed, MaxAnimationSpeed, lockProgress);

                // Check if fully locked on
                if (TimeTillLock >= n_TimeToLock)
                {
                    LockedOn = true;
                    HasTeleportedIntoRange = false; // Reset teleport flag when locked on

                    // Freeze animation on first frame
                    animator.speed = 0; // Freeze the animation
                    GetComponent<Animator>().enabled = false;
                    GetComponent<SpriteRenderer>().sprite = m_OpenEye;

                    // Start damaging player
                    if (damageCoroutine == null)
                    {
                        damageCoroutine = StartCoroutine(DamagePlayerRoutine(player));
                    }
                }
            }
        }
        else
        {
            // Player out of range, reset lock
            ResetLock();
        }
    }

    private void TeleportRandomlyAroundPlayer(Vector3 playerPosition)
    {
        // Generate a random angle
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // Generate a random distance between min and max range
        float randomDistance = Random.Range(MinTeleportRange, MaxTeleportRange);

        // Calculate new position
        Vector2 offset = new Vector2(
            Mathf.Cos(randomAngle) * randomDistance,
            Mathf.Sin(randomAngle) * randomDistance
        );

        Vector2 newPosition = (Vector2)playerPosition + offset;

        // Teleport to the new position
        transform.position = newPosition;

        // Reset lock timer to give player a fair chance
        TimeTillLock = 0f;
    }

    private void ResetLock()
    {
        LockedOn = false;
        TimeTillLock = 0f;
        HasTeleportedIntoRange = false; // Reset teleport flag when lock is reset

        // Unfreeze animation and reset speed
        animator.speed = BaseAnimationSpeed;
        GetComponent<Animator>().enabled = true; // Re-enable animator

        // Stop damage coroutine if it's running
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator DamagePlayerRoutine(GameObject player)
    {
        while (true)
        {
            // Damage the player
            PlayerController.Instance.Damage(DamagePerSecond);

            // Wait for 1 second
            yield return new WaitForSeconds(1f);
        }
    }

    protected override void OnDamage(GameObject source, float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            // Stop damage coroutine if it's running
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }

            Destroy(gameObject);
        }
    }

    // Optional: OnDestroy to ensure coroutines are stopped
    private void OnDestroy()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
    }
}
