using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObjectSpawner : NetworkBehaviour
{
    public static ObjectSpawner Instance { get; private set; }

    private Dictionary<string, int> spawnPrefabsDict = new Dictionary<string, int>();
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < MyNetworkManager.singleton.spawnPrefabs.Count; i++)
            spawnPrefabsDict.Add(MyNetworkManager.singleton.spawnPrefabs[i].name, i);
    }

    // spawning twice on client?
    [Command(requiresAuthority=false)]
    public void CmdSpawnNetworkObject(string spawnPrefabName, Vector3 pos, Quaternion rotation, NetworkConnectionToClient targetConn=null)
    {   
        GameObject go = Instantiate(MyNetworkManager.singleton.spawnPrefabs[spawnPrefabsDict[spawnPrefabName]], pos, rotation);
        NetworkServer.Spawn(go, targetConn);
    }
}
