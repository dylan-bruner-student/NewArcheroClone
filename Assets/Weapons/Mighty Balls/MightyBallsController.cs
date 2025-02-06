using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MightyBallsController : MonoBehaviour
{
    public Attribute Damage = new Attribute(25);
    public Attribute rotationSpeed = new Attribute(100);

    public GameObject centerPoint;
    private Vector3 offset;

    void Start()
    {
        if (centerPoint != null)
        {
            offset = transform.position - centerPoint.transform.position;
        }
    }

    void Update()
    {
        if (centerPoint == null) return;

        transform.position = centerPoint.transform.position + offset;
        transform.RotateAround(centerPoint.transform.position, Vector3.forward, (float)rotationSpeed.Value * Time.deltaTime);
        offset = transform.position - centerPoint.transform.position;

        var ourCollider = GetComponent<CircleCollider2D>();

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            var collider = enemy.GetComponent<Collider2D>();
            if (collider != null && ourCollider.IsTouching(collider))
            {
                enemy.GetComponent<EnemyController>().Damage((float)Damage.Value);
                Debug.Log("Hit Enemy!");
            }
        }
    }
}