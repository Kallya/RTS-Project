using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class IPDisplay : NetworkBehaviour
{
    private void Start()
    {
        if (!isServer)
            return;

        GetComponent<TMP_Text>().text = "Lobby IP: " + MyNetworkManager.singleton.networkAddress;
    }
}
