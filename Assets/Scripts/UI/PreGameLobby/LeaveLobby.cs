using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LeaveLobby : MonoBehaviour
{
    public void OnLeaveLobbyBtnClick()
    {
        if (NetworkClient.isHostClient)
        {
            NetworkManager networkRoomManager = MyNetworkManager.singleton;
            networkRoomManager.StopHost();
        }
        else
        {
            NetworkClient.localPlayer.GetComponent<NetworkIdentity>().connectionToServer.Disconnect();
        }
    }
}
