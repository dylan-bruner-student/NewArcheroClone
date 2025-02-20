using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    public void Damage(float damage)
    {
        var newDamage = PlayerController.Instance.TryCrit(damage);

        var hitMarker = Instantiate(PlayerController.Instance.HitMarker);
        hitMarker.transform.position = transform.position;

        if (newDamage != damage)
            hitMarker.GetComponentInChildren<TextMeshPro>().color = Color.red;

        OnDamage(newDamage);
    }

    protected abstract void OnDamage(float damage);
}