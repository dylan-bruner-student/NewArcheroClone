using UnityEngine;

public class BubbleController : MonoBehaviour
{
    [SerializeField] private float MovementSpeed = 7.5f;
    [SerializeField] private float EndOfLife;
    [SerializeField] private float Damage = 5f;

    private void Awake()
    {
        EndOfLife = Time.time + 10f;
    }

    private void Update()
    {
        transform.position += transform.right * MovementSpeed * Time.deltaTime;

        if (Time.time > EndOfLife)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != PlayerController.Instance.gameObject)
            return;

        PlayerController.Instance.Damage(Damage);
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}