using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DamageableObject : NetworkBehaviour
{
    private DamageableObjectStats _damageableStats;

    private void Start()
    {
        _damageableStats = GetComponent<DamageableObjectStats>();
    }

    private void Update()
    {
        // only destroy on server (since destruction is then synchronised)
        if (!isServer)
            return;

        if (_damageableStats.Health.Value <= 0)
            DestroyObject();
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
            finalDmg = dmg - charStats.Defence.Value;

        CharacterStatModifier.Instance.CmdDecreaseCharacterStat(netId, "Health", finalDmg);
        //Stats.DecreaseStat(_damageableStats.Health, finalDmg);

        // change health on clients
        //RpcTakeDamage(finalDmg);
    }
/*
    [ClientRpc]
    private void RpcTakeDamage(int dmg)
    {
        if (isServer)
            return;

        Stats.DecreaseStat(_damageableStats.Health, dmg);
    }
*/

    private void OnCollisionEnter(Collision other)
    {
        if (!isServer)
            return;

        if (other.gameObject.TryGetComponent<IWeapon>(out IWeapon weaponCollision))
            TakeDamage(weaponCollision.Damage);
    }
}
