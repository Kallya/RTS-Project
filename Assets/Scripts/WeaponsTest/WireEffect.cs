using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WireEffect : NetworkBehaviour
{
    private static int _speedDecrease;

    private void OnTriggerEnter(Collider coll)
    {
        if (!isServer)
            return;

        NetworkIdentity collNetIdentity;

        // no effect if character is part of same team that deployed wire
        if (coll.TryGetComponent<NetworkIdentity>(out collNetIdentity))
            if (collNetIdentity.connectionToClient.connectionId == connectionToClient.connectionId)
                return;
        else
            return;

        if (coll.TryGetComponent<CharacterStats>(out CharacterStats charStats))
        {
            uint characterNetId = collNetIdentity.netId;

            CharacterStatModifier.Instance.DecreaseCharacterStat(characterNetId, "Speed", _speedDecrease);
            CharacterStatModifier.Instance.RpcDecreaseCharacterStat(characterNetId, "Speed", _speedDecrease);
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (!isServer)
            return;

        NetworkIdentity collNetIdentity;

        // no effect if character is part of same team that deployed wire
        if (coll.TryGetComponent<NetworkIdentity>(out collNetIdentity))
            if (collNetIdentity.connectionToClient.connectionId == connectionToClient.connectionId)
                return;
        else
            return;

        if (coll.TryGetComponent<CharacterStats>(out CharacterStats charStats))
        {
            uint characterNetId = collNetIdentity.netId;

            CharacterStatModifier.Instance.IncreaseCharacterStat(characterNetId, "Speed", _speedDecrease);
            CharacterStatModifier.Instance.RpcIncreaseCharacterStat(characterNetId, "Speed", _speedDecrease);
        }
    }
}
