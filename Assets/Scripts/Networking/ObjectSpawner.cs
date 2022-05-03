using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObjectSpawner : NetworkBehaviour
{
    public static ObjectSpawner Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
    }

    // spawning twice on client?
    [Command(requiresAuthority=false)]
    /// <param name="spawnPrefab">object to spawn registered in NetworkManager spawnable prefabs</param>
    /// <param name="pos">position to spawn at in 3d space</param>
    /// <param name="rotation">object rotation at spawn</param>
    /// <param name="targetConn">client connection to server</param>
    public void CmdSpawnNetworkObject(GameObject spawnPrefab/*int spawnPrefabIndex*/, Vector3 pos, Quaternion rotation, NetworkConnectionToClient targetConn=null)
    {   
        // GameObject go = Instantiate(MyNetworkManager.singleton.spawnPrefabs[spawnPrefabIndex], pos, rotation);
        GameObject go = Instantiate(spawnPrefab, pos, rotation);
        NetworkServer.Spawn(go, targetConn);
    }
}
