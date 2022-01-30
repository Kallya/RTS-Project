using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct CloakMessage : NetworkMessage
{
    public uint CharacterNetId;
    public bool IsCloaked;
}
public class CloakButton : MonoBehaviour
{
    public void OnCloakBtnClick()
    {
        GameObject currCharacter = PlayerInfoUIManager.Instance.CurrCharacter;

        PlayerCommandInput currInput = currCharacter.GetComponent<PlayerCommandInput>();
        currInput.IsCloaked = !currInput.IsCloaked;

        CloakMessage msg = new CloakMessage()
        {
            CharacterNetId=currCharacter.GetComponent<NetworkIdentity>().netId,
            IsCloaked=currInput.IsCloaked
        };

        NetworkClient.Send(msg);
    }
}
