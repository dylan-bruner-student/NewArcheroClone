using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnifeManager : BaseWeaponManager
{
    [SerializeField] public GameObject ThrowingKnifePrefab;

    private float LastSpawnTime = 0;
    public Attribute SpawnDelay = new Attribute(0.25);
    public Attribute AttackRange = new Attribute(25);

    void Update()
    {

        if (LastSpawnTime + SpawnDelay.Value < Time.time)
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
            if (distance < AttackRange.Value && distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}
