using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowingKnifeController : BaseWeaponController
{
    [SerializeField] public float MovementSpeed = 40;
    [SerializeField] public float Damage = 25;

    void Start()
    {
    }

    void Update()
    {
        if (Target == null || Target.gameObject.IsDestroyed())
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (Target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        Vector3 move = Vector3.MoveTowards(transform.position, Target.transform.position, Time.deltaTime * MovementSpeed);
        transform.position = move;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var controller = collision.gameObject.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.Damage(Damage);
            Destroy(gameObject);
        }
    }
}
