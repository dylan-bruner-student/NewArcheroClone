using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowingKnifeController : BaseWeaponController
{
    [SerializeField] private Attribute MovementSpeed = new Attribute(40);
    [SerializeField] private Attribute Damage = new Attribute(25);

    void Start()
    {
    }

    void Update()
    {
        if (Target == null || Target.gameObject.IsDestroyed()) {
            Destroy(gameObject);
            return;
        }

        Vector3 move = Vector3.MoveTowards(transform.position, Target.transform.position, Time.deltaTime * (float) MovementSpeed.Value);
        transform.position = move;
        transform.LookAt(transform.position);

        var targetCollider = Target.GetComponent<Collider2D>();
        if (targetCollider == null)
        {
            Debug.LogWarning("The target doesn't have a 2D collider!!!");
            return;
        }

        if (GetComponent<BoxCollider2D>().IsTouching(targetCollider))
        {
            Target.GetComponent<EnemyController>().Damage((float) Damage.Value);
            Destroy(gameObject);
        }
    }
}
