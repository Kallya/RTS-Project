using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
            DestroyObject();
    }

    // override to do other stuff on death (send notification?)
    public virtual void DestroyObject()
    {
        Destroy(gameObject);
    }

    // Security wise stats should be updated on the server only and synchronised from there
    // but I don't know how to synchronised my Stat object values so yeah
    public void TakeDamage(int dmg)
    {
        if (!isServer)
            return;

        int finalDmg = dmg;

        if (_damageableStats is CharacterStats charStats)
            finalDmg = dmg - charStats.Defence.Value;

        Stats.DecreaseStat(_damageableStats.Health, finalDmg);

        // change health on clients
        RpcTakeDamage(finalDmg);
    }

    [ClientRpc]
    private void RpcTakeDamage(int dmg)
    {
        if (isServer)
            return;

        Stats.DecreaseStat(_damageableStats.Health, dmg);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<IWeapon>(out IWeapon weaponCollision))
            TakeDamage(weaponCollision.Damage);
    }
}
