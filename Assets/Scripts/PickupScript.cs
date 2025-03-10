using UnityEngine;

public class PickupScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            Destroy(collision.gameObject);
            PlayerController.Instance.AddScore(25);
        }
    }
}
