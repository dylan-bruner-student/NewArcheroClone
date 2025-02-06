using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float MovementSpeed = 10f;
    [SerializeField] private GameObject enemy;
    [SerializeField] private Image HealthBar;

    [SerializeField] private float MaxHealth = 1000f;
    [SerializeField] private float Health = 1000f;

    [Header("Weapons")]
    [SerializeField] private GameObject ThrowingKnife;


    void Start() {
        gameObject.AddComponent<ThrowingKnifeManager>();
        gameObject.GetComponent<ThrowingKnifeManager>().ThrowingKnifePrefab = ThrowingKnife;

        gameObject.AddComponent<MightyBallsManager>();
    }


    void Update() {
        Vector3 velocity = Vector3.zero;


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            velocity.y = MovementSpeed;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            velocity.y = -MovementSpeed;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            velocity.x = -MovementSpeed;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            velocity.x = MovementSpeed;

        if (Input.GetKey(KeyCode.Space)) {
            var thing = Instantiate(enemy);
            thing.transform.position = new Vector3(Random.Range(-25, 25), Random.Range(-25, 25));
        }

        var rb = GetComponent<Rigidbody2D>();
        rb.velocity = velocity;
    }


    public void Damage(int damage)
    {
        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);
        HealthBar.fillAmount = (Health / 1000);
    }
}
