using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusMineController : EnemyController
{
    public override bool Targetable => false;

    [SerializeField] private float Health = 1000f;
    [SerializeField] public static float HitDamage = 10f;
    [SerializeField] private GameObject Needle;

    private int NeedleCount = 50;

    protected override void OnDamage(GameObject source, float damage) {
        if (source.GetComponent<MightyBallsController>() == null)
            return;

        if (UpgradeController.Instance.HasUpgrade(typeof(Incinerator)))
        {
            Health -= damage;
            if (Health <= 0)
                Destroy(gameObject);
        }
    }

    private void SpawnNeedles()
    {
        // Calculate angle between each needle
        float angleStep = 360f / NeedleCount;
        float radius = 0.25f; // Distance from center to spawn needles
        float needleForce = 15f; // Force to apply to needles

        for (int i = 0; i < NeedleCount; i++)
        {
            // Calculate position in circle
            float angle = i * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            Vector2 spawnPosition = transform.position + new Vector3(
                radius * Mathf.Cos(radians),
                radius * Mathf.Sin(radians),
                0
            );

            // Spawn the needle
            GameObject needle = Instantiate(Needle, spawnPosition, Quaternion.identity);

            // Set the needle rotation to face outward
            float rotationAngle = angle - 90; // Adjust based on your needle sprite orientation
            needle.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

            // Get the rigidbody and apply force
            Rigidbody2D rb = needle.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Calculate direction from center to needle
                Vector2 forceDirection = (spawnPosition - (Vector2)transform.position).normalized;

                // Apply force in outward direction
                rb.AddForce(forceDirection * needleForce, ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogWarning("Needle prefab is missing a Rigidbody2D component!");
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerController.Instance.gameObject)
        {
            SpawnNeedles();
            Destroy(gameObject);
        }
    }
}
