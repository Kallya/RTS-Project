using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SetReadyState : MonoBehaviour
{
    public void ReadyBtnClick()
    {
        MyNetworkRoomPlayer roomPlayer = NetworkClient.localPlayer.GetComponent<MyNetworkRoomPlayer>();

        roomPlayer.CmdChangeReadyState(!roomPlayer.readyToBegin);
    }
}
