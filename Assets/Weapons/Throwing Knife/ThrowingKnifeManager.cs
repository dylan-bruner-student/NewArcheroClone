using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnifeManager : BaseWeaponManager
{
    [SerializeField] public GameObject ThrowingKnifePrefab;

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
        if (PlayerController.Instance.IsMoving)
            return;

        if (LastSpawnTime + SpawnDelay < Time.time)
        {
            LastSpawnTime = Time.time;
            var gameObj = Instantiate(ThrowingKnifePrefab);
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
