using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SetReadyState : MonoBehaviour
{
    public void OnReadyBtnClick()
    {
        NetworkClient.localPlayer.GetComponent<MyNetworkRoomPlayer>().CmdChangeReadyState(true);
    }
}
