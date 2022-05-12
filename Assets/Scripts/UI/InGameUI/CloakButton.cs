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
        PlayerInfoUIManager.Instance.CurrCmdInput.ChangeCloak();
    }
}
