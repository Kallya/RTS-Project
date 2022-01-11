using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// empty message to trigger UI enable for character setup
public struct StartPreGameMessage : NetworkMessage {}

public class MyNetworkManager : NetworkRoomManager
{  
    public int CharacterNum { get; set; } = 0;

    private string[][] _characterWeaponSelection;
    private UIObjectReferences _ui;

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        if (sceneName == RoomScene)
        {
            NetworkServer.RegisterHandler<WeaponSelectionMessage>(OnNetworkLockIn);

            // disable UI because it's automatically enabled on spawn
            _ui = GameObject.Find("UIObjectReferences").GetComponent<UIObjectReferences>();
            _ui.CharacterSetupUI.SetActive(false);
            _ui.EventSystem.SetActive(false);
        }   
    }

    public override void OnRoomStartClient()
    {
        NetworkClient.RegisterHandler<StartPreGameMessage>(OnStartPreGame);
    }

    // synchronise character setup start on clients (not just server)
    public void OnStartPreGame(StartPreGameMessage msg)
    {
        showRoomGUI = false;
        
        UIObjectReferences _ui = GameObject.Find("UIObjectReferences").GetComponent<UIObjectReferences>();
        _ui.CharacterSetupUI.SetActive(true);
        _ui.EventSystem.SetActive(true);
    }

    public override void OnRoomServerPlayersReady()
    {
        StartPreGameMessage msg = new StartPreGameMessage();
        NetworkServer.SendToReady(msg);
    }

    public override void OnRoomClientExit()
    {
        NetworkClient.UnregisterHandler<StartPreGameMessage>();
    }

    private void OnAllLockIn()
    {
        NetworkServer.UnregisterHandler<WeaponSelectionMessage>();
        ServerChangeScene(GameplayScene);
    }

    public void OnNetworkLockIn(NetworkConnection conn, WeaponSelectionMessage msg)
    {  
        // player configs are null if no character
        // they won't be accessed so there won't be a problem
        _characterWeaponSelection = new string[4][]
            {
                msg.Config1,
                msg.Config2,
                msg.Config3,
                msg.Config4
            };
        
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

        string[] weaponSelection = _characterWeaponSelection[playerIndex];

        foreach (string weapon in weaponSelection)
            playerWeapons.WeaponsToAdd.Add(weapon);
    }
}
