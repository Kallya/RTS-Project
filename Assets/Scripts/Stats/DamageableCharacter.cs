using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DamageableCharacter : DamageableObject
{
    public event System.Action<GameObject> OnDestroyed;
    private int _lastCollConnId;
    
    private void OnCollisionEnter(Collision coll)
    {
        // only do something for collisions on server
        if (!isServer)
            return;

        _lastCollConnId = GetCollId(coll);

        if (coll.gameObject.TryGetComponent<IWeapon>(out IWeapon weaponCollision))
            TakeDamage(weaponCollision.Damage);

        if (_damageableStats.Health.Value <= 0)
            DestroyObject();
    }

    private void DestroyCharacterSetup(int attackerConnId, int thisConnId)
    {
        ScoreManager.Instance.UpdateScore(attackerConnId, thisConnId);
        OnDestroyed?.Invoke(gameObject);
    }

    public override void DestroyObject()
    {
        int thisConnId = connectionToClient.connectionId;

        DestroyCharacterSetup(_lastCollConnId, thisConnId); // update on server
        RpcDestroyCharacterSetup(_lastCollConnId, thisConnId); // update on client

        base.DestroyObject();
    }

    [ClientRpc]
    private void RpcDestroyCharacterSetup(int attackerConnId, int thisConnId)
    {
        if (isServer)
            return;
            
        DestroyCharacterSetup(attackerConnId, thisConnId);
    }

    // two cases
    // weapon is separated from player (e.g. bullet)
    // weapon is equipped by player (e.g. sword)
    private int GetCollId(Collision coll)
    {
        if (coll.gameObject.TryGetComponent<NetworkIdentity>(out NetworkIdentity id))
            return id.connectionToClient.connectionId;
        else
            return coll.transform.root.GetComponent<NetworkIdentity>().connectionToClient.connectionId;
    }

    // functionality if destroyed via means other than loss of health
    // ie. by bail out
    private void OnDestroy()
    {
        // isclientonly instead of !isserver
        // cause !isserver causing errors allowing client to run RPC
        if (isClientOnly)
            return;

        if (!isServer)
            return;

        if (_damageableStats.Health.Value <= 0)
            return;

        int thisConnId = connectionToClient.connectionId;

        // equivalent to self kill, no score rewarded
        // so connId is the same
        DestroyCharacterSetup(thisConnId, thisConnId);
        RpcDestroyCharacterSetup(thisConnId, thisConnId);
    }
}
