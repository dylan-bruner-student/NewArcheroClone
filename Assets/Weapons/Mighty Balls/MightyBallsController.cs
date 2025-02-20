using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MightyBallsController : MonoBehaviour
{
    [SerializeField] public float Damage = 25;
    [SerializeField] public float rotationSpeed = -100;
    [SerializeField] public GameObject centerPoint;

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
        transform.RotateAround(centerPoint.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        offset = transform.position - centerPoint.transform.position;
        transform.Rotate(Vector3.forward, (rotationSpeed * 6) * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<EnemyController>()?.Damage(Damage);
    }
}