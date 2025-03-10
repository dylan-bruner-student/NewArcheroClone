using UnityEngine;

public class ThrowingKnifeManager : BaseWeaponManager
{
    private float LastSpawnTime = 0;

    [SerializeField] public float SpawnDelay = 0.25f;
    [SerializeField] public float AttackRange = 25;

    public static ThrowingKnifeManager Instance;

    private void Start()
    {
        Instance = this;
    }

    void Update()
    {
        if (LastSpawnTime + SpawnDelay < Time.time)
        {
            LastSpawnTime = Time.time;
            var gameObj = Instantiate(PlayerController.Instance.ThrowingKnife);
            gameObj.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
            gameObj.GetComponent<BaseWeaponController>().Target = FindNearestEnemyInRange();
        }
    }

    private GameObject FindNearestEnemyInRange()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (player == null || enemies.Length == 0)
            return null;

        GameObject nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (enemy.GetComponent<EnemyController>()?.Targetable != true)
                continue;

            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (distance < AttackRange && distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}
