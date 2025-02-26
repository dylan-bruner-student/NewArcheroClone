using UnityEngine;

public class SquareManController : EnemyController
{
    private float Health = 100f;
    private float MovementSpeed = 15f;

    protected override void OnDamage(GameObject source, float damage)
    {
        Health -= damage;
        if (Health <= 0)
            Destroy(gameObject);
    }

    private void Update()
    {
        var target = PlayerController.Instance.transform;
        Vector2 direction = (target.position - transform.position).normalized;
        GetComponent<Rigidbody2D>().velocity = direction * MovementSpeed;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == PlayerController.Instance.gameObject)
        {
            PlayerController.Instance.Damage(25f);
            Destroy(gameObject);
        }
    }
}