using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DamageableObject : MonoBehaviour
{
    public event System.Action<GameObject> OnDestroyed;
    private DamageableObjectStats _damageableStats;

    private void Awake()
    {
        _damageableStats = GetComponent<DamageableObjectStats>();
    }

    private void Update()
    {
        if (_damageableStats.Health.Value <= 0)
            DestroyObject();
    }

    // override to do other stuff on death (send notification?)
    public virtual void DestroyObject()
    {
        Destroy(gameObject);
        OnDestroyed?.Invoke(gameObject);
    }

    // Security wise stats should be updated on the server only and synchronised from there
    // but I don't know how to synchronised my Stat object values so yeah
    private void TakeDamage(int damage)
    {
        int finalDmg = damage;

        if (_damageableStats is CharacterStats charStats)
            finalDmg = damage - charStats.Defence.Value;

        Stats.DecreaseStat(_damageableStats.Health, finalDmg);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<IWeapon>(out IWeapon weaponCollision))
            TakeDamage(weaponCollision.Damage);
    }
}
