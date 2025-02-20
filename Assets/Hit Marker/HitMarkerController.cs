using UnityEngine;

public class HitMarkerController : MonoBehaviour
{
    private float DieAt;

    private void Start()
    {
        DieAt = Time.time + 1f;
    }

    private void Update()
    {
        if (Time.time > DieAt)
            Destroy(gameObject);
    }
}