using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : EnemyController
{
    [SerializeField] private float ShotDelay = 1.25f;
    [SerializeField] private float AttackRange = 15f;

    [SerializeField] private float Health = 100f;
    [SerializeField] private float LastFireTime = 0;

    [SerializeField] private GameObject m_FishProjectile;
    [SerializeField] private float moveSpeed = 2f;


    private void Start()
    {
        StartCoroutine(BobAnimation());
    }

    void Update()
    {
        var player = PlayerController.Instance.gameObject;
        Vector2 direction = (player.transform.position - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

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

    protected override void OnDamage(float damage)
    {
        Health = Mathf.Max(Health - damage, 0);

        if (Health <= 0)
            Destroy(gameObject);
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