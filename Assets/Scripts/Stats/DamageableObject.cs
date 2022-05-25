using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DamageableObject : NetworkBehaviour
{
    protected DamageableObjectStats _damageableStats;

    private void Start()
    {
        _damageableStats = GetComponent<DamageableObjectStats>();
    }

    // override to do other stuff on death (send notification?)
    public virtual void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(int dmg)
    {
        int finalDmg = dmg;

        if (_damageableStats is CharacterStats charStats)
        {
            if (IsCrit())
                finalDmg *= 2;
        
            finalDmg -= charStats.Defence.Value;
        }

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
        if (!isServer)
            return;

        if (other.gameObject.TryGetComponent<IWeapon>(out IWeapon weaponCollision))
            TakeDamage(weaponCollision.Damage);

        if (_damageableStats.Health.Value <= 0)
            DestroyObject();
    }

    // rng crit component (10% fixed chance)
    private bool IsCrit()
    {
        int n = Random.Range(1, 11);

        if (n == 1)
            return true;
        else
            return false;
    }
}
