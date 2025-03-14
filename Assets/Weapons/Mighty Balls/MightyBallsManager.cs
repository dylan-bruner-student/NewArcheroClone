using System.Collections.Generic;
using UnityEngine;

public class MightyBallsManager : BaseWeaponManager
{
    [SerializeField] public int BallCount = 0;
    [SerializeField] public float Offset = 2f;
    [SerializeField] public float BallDamage = 1.0f;

    public static MightyBallsManager Instance;

    private List<GameObject> ball_list = new List<GameObject>();

    public void Refresh()
    {
        ClearBalls();
        SpawnBalls();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
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

        bool incin = UpgradeController.Instance.HasUpgrade(typeof(Incinerator));

        for (int i = 0; i < BallCount; i++)
        {
            float angle = i * (2 * Mathf.PI / BallCount); // Distribute evenly around a circle
            Vector3 spawnPosition = playerObject.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Offset;

            var ball = Instantiate(PlayerController.Instance.MightyBall, spawnPosition, Quaternion.identity);

            if (incin)
                ball.GetComponentInChildren<SpriteRenderer>().color = new Color(1.0f, 178f / 255f, 0);

            var controller = ball.AddComponent<MightyBallsController>();
            controller.centerPoint = playerObject;
            controller.Damage = BallDamage;

            ball_list.Add(ball);
        }
    }
}