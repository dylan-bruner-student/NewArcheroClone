using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float Health = 100f;
    [SerializeField] private float MovementSpeed = 10f;


    void Start()
    {
        
    }

    void Update()
    {
        var target = FindFirstObjectByType<PlayerController>().transform.position;
        var rb = GetComponent<Rigidbody2D>();

        Vector2 direction = (target - transform.position);
        rb.velocity = direction * 2f;

        var player = FindFirstObjectByType<PlayerController>();
        if (player.GetComponentInChildren<CircleCollider2D>().IsTouching(gameObject.GetComponent<CircleCollider2D>()))
        {
            player.GetComponent<PlayerController>().Damage(5);
            Destroy(gameObject);
        }
        
    }

    public void Damage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            Destroy(gameObject);
    }
}
