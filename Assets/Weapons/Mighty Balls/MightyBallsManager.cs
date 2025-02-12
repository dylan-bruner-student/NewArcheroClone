using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MightyBallsManager : BaseWeaponManager
{
    [SerializeField] public GameObject BallRefrence;
    [SerializeField] public int BallCount = 4;
    [SerializeField] public float Offset = 2f;

    public static MightyBallsManager Instance;

    private List<GameObject> ball_list = new List<GameObject>();

    public void Refresh()
    {
        ClearBalls();
        SpawnBalls();
    }

    private void Start()
    {
        Instance = this;
        SpawnBalls();
    }

    private void OnValidate()
    {
        Refresh();
    }

    private void ClearBalls()
    {
        foreach (var ball in ball_list)
            Destroy(ball);
        ball_list.Clear();
    }

    private void SpawnBalls()
    {
        ClearBalls();
        var playerObject = PlayerController.Instance.gameObject;

        for (int i = 0; i < BallCount; i++)
        {
            float angle = i * (2 * Mathf.PI / BallCount); // Distribute evenly around a circle
            Vector3 spawnPosition = playerObject.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Offset;

            var ball = Instantiate(BallRefrence, spawnPosition, Quaternion.identity);
            var controller = ball.AddComponent<MightyBallsController>();
            controller.centerPoint = playerObject;

            ball_list.Add(ball);
        }
    }
}