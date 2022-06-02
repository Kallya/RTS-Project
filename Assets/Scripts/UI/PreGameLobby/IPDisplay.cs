using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Mirror;
using TMPro;

public class IPDisplay : NetworkBehaviour
{
    private void Start()
    {
        if (!isServer)
            return;

        GetComponent<TMP_Text>().text = "Lobby IP: " + GetLocalIP();
    }

    private string GetLocalIP()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "localhost";
    }
}
