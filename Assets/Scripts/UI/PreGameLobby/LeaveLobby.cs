using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LeaveLobby : MonoBehaviour
{
    public void LeaveLobbyBtnClick()
    {
        if (NetworkClient.isHostClient)
        {
            NetworkManager networkRoomManager = MyNetworkManager.singleton;
            networkRoomManager.StopHost();
        }
        else
            MyNetworkManager.singleton.StopClient();
    }
}
