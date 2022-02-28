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
    public void CmdSpawnNetworkObject(int spawnPrefabIndex, Vector3 pos, Quaternion rotation, NetworkConnectionToClient targetConn=null)
    {   
        GameObject go = Instantiate(MyNetworkManager.singleton.spawnPrefabs[spawnPrefabIndex], pos, rotation);
        NetworkServer.Spawn(go, targetConn);
    }
}
