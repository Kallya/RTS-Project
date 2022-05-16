using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WireEffect : NetworkBehaviour
{
    private static int _speedDecrease = 3;

    private void OnTriggerEnter(Collider coll)
    {
        if (!isServer)
            return;

        // no effect if character is part of same team that deployed wire
        if (coll.TryGetComponent<NetworkIdentity>(out NetworkIdentity collNetIdentity))
            if (collNetIdentity.connectionToClient.connectionId == connectionToClient.connectionId)
                return;
        else
            return;

        if (coll.TryGetComponent<CharacterStats>(out CharacterStats charStats))
        {
            uint characterNetId = collNetIdentity.netId;

            CharacterStatModifier.Instance.CmdDecreaseCharacterStat(characterNetId, "Speed", _speedDecrease);
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (!isServer)
            return;

        // no effect if character is part of same team that deployed wire
        if (coll.TryGetComponent<NetworkIdentity>(out NetworkIdentity collNetIdentity))
            if (collNetIdentity.connectionToClient.connectionId == connectionToClient.connectionId)
                return;
        else
            return;

        if (coll.TryGetComponent<CharacterStats>(out CharacterStats charStats))
        {
            uint characterNetId = collNetIdentity.netId;

            CharacterStatModifier.Instance.CmdIncreaseCharacterStat(characterNetId, "Speed", _speedDecrease);
        }
    }
}
