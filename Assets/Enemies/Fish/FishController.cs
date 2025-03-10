using System.Collections;
using UnityEngine;

public class FishController : EnemyController
{
    [SerializeField] private float ShotDelay = 1.25f;
    [SerializeField] private float AttackRange = 15f;

    [SerializeField] private float Health = 100f;
    [SerializeField] private float LastFireTime = 0;

    [SerializeField] private GameObject m_FishProjectile;
    [SerializeField] private float moveSpeed = 2f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        StartCoroutine(BobAnimation());
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If we can't find the sprite renderer on this object, try finding it on a child
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void Update()
    {
        var player = PlayerController.Instance.gameObject;
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Store the angle for projectile direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Flip the sprite based on direction using the SpriteRenderer
        if (spriteRenderer != null)
        {
            if (direction.x > 0)
            {
                // Player is to the right, face right
                spriteRenderer.flipX = false;
            }
            else
            {
                // Player is to the left, face left
                spriteRenderer.flipX = true;
            }
        }

        var distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance > 5f)
        {
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }

        if (distance < AttackRange && Time.time - ShotDelay > LastFireTime)
        {
            var proj = Instantiate(m_FishProjectile);
            proj.transform.position = transform.position;
            proj.transform.rotation = Quaternion.Euler(0, 0, angle);

            LastFireTime = Time.time;
        }
    }

    protected override void OnDamage(GameObject source, float damage)
    {
        Health = Mathf.Max(Health - damage, 0);

        if (Health <= 0)
        {
            Destroy(gameObject);
            var coin = Instantiate(PlayerController.Instance.CoinDrop);
            coin.transform.position = transform.position;
        }
    }

    IEnumerator BobAnimation()
    {
        while (true)
        {
            transform.Translate(new Vector3(0, 0.1f, 0));
            yield return new WaitForSeconds(0.2f);
            transform.Translate(new Vector3(0, -0.1f, 0));
            yield return new WaitForSeconds(0.2f);
        }
    }
}
