using UnityEngine;

public class CactusProjectileController : MonoBehaviour
{
    private void Start()
    {
        Invoke("Kill", 2);
    }

    private void Kill()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerController.Instance.gameObject)
        {
            PlayerController.Instance.Damage(CactusMineController.HitDamage);
            Destroy(gameObject);
        }
    }
}