using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkRoomManager
{  
    public int CharacterNum { get; set; } = 0;

    private string[][] _characterWeaponSelection;

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        if (sceneName == RoomScene)
            NetworkServer.RegisterHandler<WeaponSelectionMessage>(OnNetworkLockIn);
    }

    public override void OnRoomServerPlayersReady()
    {
        showRoomGUI = false;
        UIObjectReferences ui = GameObject.Find("UIObjectReferences").GetComponent<UIObjectReferences>();
        ui.CharacterSetupUI.SetActive(true);
        ui.EventSystem.SetActive(true);
    }

    private void OnAllLockIn()
    {
        ServerChangeScene(GameplayScene);
    }

    public void OnNetworkLockIn(NetworkConnection conn, WeaponSelectionMessage msg)
    {  
        _characterWeaponSelection = msg.weaponConfigs;
        conn.identity.gameObject.GetComponent<MyNetworkRoomPlayer>().LockedIn = true;
        
        // check if all players are locked in
        foreach (NetworkRoomPlayer player in roomSlots)
        {
            MyNetworkRoomPlayer mPlayer = player as MyNetworkRoomPlayer;
            
            if (!mPlayer.LockedIn)
                return;
        }

        OnAllLockIn();
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        Vector3 startPos = GetRandomStartPos(0, 50);
        GameObject player = Instantiate(playerPrefab, startPos, Quaternion.identity);

        AssignWeapons(player, 0);

        return player;
    }
    
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        // Spawn additional characters
        for (int i = 1; i < CharacterNum; i++)
        {
            Vector3 startPos = GetRandomStartPos(0, 50);
            GameObject character = SpawnNetworkObject(playerPrefab, startPos, Quaternion.identity, conn);

            AssignWeapons(character, i);
        }

        return true;
    }

    private GameObject SpawnNetworkObject(GameObject prefab, Vector3 pos, Quaternion rotation, NetworkConnection targetConn=null)
    {   
        GameObject go = Instantiate(prefab, pos, rotation);
        go.name = $"{go.name} [connId={targetConn?.connectionId}]";
        NetworkServer.Spawn(go, targetConn);

        return go;
    }

    private Vector3 GetRandomStartPos(int minVal, int maxVal)
    {
        return new Vector3(Random.Range(minVal, maxVal), 0f, Random.Range(minVal, maxVal));
    }

    private void AssignWeapons(GameObject character, int playerIndex)
    {
        PlayerWeapons playerWeapons = character.GetComponent<PlayerWeapons>();
        playerWeapons.WeaponsToAdd = _characterWeaponSelection[playerIndex];
        playerWeapons.enabled = true;
    }
}
