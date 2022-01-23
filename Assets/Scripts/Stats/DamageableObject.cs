using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(DamageableObjectStats))]
public class DamageableObject : NetworkBehaviour
{
    private DamageableObjectStats _damageableStats;

    private void Awake()
    {
        _damageableStats = GetComponent<DamageableObjectStats>();
    }

    private void Update()
    {
        if (_damageableStats.Health.Value <= 0)
            CmdDestroyObject();
    }

    // override to do other stuff on death (send notification?)
    [Command]
    public virtual void CmdDestroyObject()
    {
        Destroy(gameObject);
    }

    // Security wise stats should be updated on the server only and synchronised from there
    // but I don't know how to synchronised my Stat object values so yeah
    private void TakeDamage(int damage)
    {
        Stats.DecreaseStat(_damageableStats.Health, damage);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<IWeapon>(out IWeapon weaponCollision))
        {
            TakeDamage(weaponCollision.Damage);
            Debug.Log($"{gameObject.name} took {weaponCollision.Damage} damage");
        }
    }
}
