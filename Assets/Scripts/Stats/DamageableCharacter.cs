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
            TakeDamage(weaponCollision.Damage);
    }

    public override void DestroyObject()
    {
        ScoreManager.Instance.UpdateScore(_lastCollConnId, connectionToClient.connectionId);
        OnDestroyed?.Invoke(gameObject);
        
        base.DestroyObject();
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
