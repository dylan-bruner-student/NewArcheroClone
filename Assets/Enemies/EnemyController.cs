using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    public System.Action<GameObject> OnDeath;

    public virtual bool Targetable { get; } = true;

    public void Damage(GameObject source, float damage)
    {
        var newDamage = PlayerController.Instance.TryCrit(damage);

        var hitMarker = Instantiate(PlayerController.Instance.HitMarker);
        hitMarker.transform.position = transform.position;

        if (newDamage != damage)
            hitMarker.GetComponentInChildren<TextMeshPro>().color = Color.red;

        OnDamage(source, newDamage);
    }

    protected abstract void OnDamage(GameObject source, float damage);

    private void OnDestroy()
    {
        OnDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }
}