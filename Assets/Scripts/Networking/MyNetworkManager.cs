using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{   
    // need dynamic implementation for number of characters to spawn
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // Spawn main player character (because mirror only allows one player gameObject)
        base.OnServerAddPlayer(conn);

        // Spawn additional characters
        for (int i = 1; i < 4; i++)
        {
            Vector3 startPos = new Vector3(Random.Range(0, 50), 0f, Random.Range(0, 50));
            SpawnNetworkObject(playerPrefab, startPos, Quaternion.identity, conn);
        }
    }

    private void SpawnNetworkObject(GameObject prefab, Vector3 pos, Quaternion rotation, NetworkConnection targetConn=null)
    {   
        GameObject obj = Instantiate(prefab, pos, rotation);
        obj.name = $"{obj.name} [connId={targetConn?.connectionId}]";
        NetworkServer.Spawn(obj, targetConn);
    }
}
