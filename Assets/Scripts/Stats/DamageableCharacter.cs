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
        _lastCollConnId = GetCollId(coll);

        if (coll.gameObject.TryGetComponent<IWeapon>(out IWeapon weaponCollision))
            ServerTakeDamage(weaponCollision.Damage);
    }

    private void DestroyCharacterSetup(int attackerConnId, int thisConnId)
    {
        ScoreManager.Instance.UpdateScore(_lastCollConnId, thisConnId);
        OnDestroyed?.Invoke(gameObject);
    }

    public override void ServerDestroyObject()
    {
        if (!isServer)
            return;

        // destroyed object (this) connId stored here
        // cause destroyed before DestroyCharacterSetup called
        // leading to null object ref when accessing connectionToClient
        int thisConnId = connectionToClient.connectionId;

        DestroyCharacterSetup(_lastCollConnId, thisConnId);
        RpcDestroyObject(_lastCollConnId, thisConnId);

        base.ServerDestroyObject();
    }

    [ClientRpc]
    private void RpcDestroyObject(int attackerConnId, int thisConnId)
    {
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
            return coll.transform.parent.GetComponent<NetworkIdentity>().connectionToClient.connectionId;
    }
}
