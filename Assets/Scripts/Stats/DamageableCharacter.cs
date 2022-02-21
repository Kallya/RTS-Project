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
    }

    private void DestroyCharacterSetup(int attackerConnId, int thisConnId)
    {
        ScoreManager.Instance.UpdateScore(_lastCollConnId, thisConnId);
        OnDestroyed?.Invoke(gameObject);
    }

    public override void DestroyObject()
    {
        int thisConnId = connectionToClient.connectionId;

        DestroyCharacterSetup(_lastCollConnId, thisConnId); // update on server
        RpcDestroyObject(_lastCollConnId, thisConnId); // update on client

        base.DestroyObject();
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
