using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// empty message to trigger UI enable for character setup
public struct StartPreGameMessage : NetworkMessage {}
// empty message to tell clients when to call SetLocalCharacters
// to ensure all characters (allied and enemy) are setup
public struct SetLocalCharactersMessage : NetworkMessage {}

public class MyNetworkManager : NetworkRoomManager
{  
    [SerializeField] private GameObject emptyPlayerPrefab;
    private int _totalCharacterNum = 0;
    private int _currSpawnedCharacterNum = 0;

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        if (sceneName == RoomScene)
        {
            NetworkServer.RegisterHandler<WeaponSelectionMessage>(OnNetworkLockIn);

            // disable UI because it's automatically enabled on spawn
            UIObjectReferences.Instance.CharacterSetupUI.SetActive(false);
            UIObjectReferences.Instance.EventSystem.SetActive(false);
        }   
    }

    public override void OnRoomStartClient()
    {
        NetworkClient.RegisterHandler<StartPreGameMessage>(OnStartPreGame);
        NetworkClient.RegisterHandler<SetLocalCharactersMessage>(OnSetLocalCharacters);
    }

    // synchronise character setup start on clients (not just server)
    private void OnStartPreGame(StartPreGameMessage msg)
    {
        showRoomGUI = false;
        
        UIObjectReferences _ui = GameObject.Find("UIObjectReferences").GetComponent<UIObjectReferences>();
        _ui.CharacterSetupUI.SetActive(true);
        _ui.EventSystem.SetActive(true);
    }

    private void OnSetLocalCharacters(SetLocalCharactersMessage msg)
    {
        POVManager.Instance.SetLocalCharacters();
        NetworkClient.UnregisterHandler<SetLocalCharactersMessage>();
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
        string[][] characterWeaponSelection = new string[4][]
            {
                msg.Config1,
                msg.Config2,
                msg.Config3,
                msg.Config4
            };

        int characterNum = 0;
        foreach (string[] config in characterWeaponSelection)
        {
            if (config != null)
                characterNum += 1;
        }

        _totalCharacterNum += characterNum;

        MyNetworkRoomPlayer currRoomPlayer = conn.identity.gameObject.GetComponent<MyNetworkRoomPlayer>();
        currRoomPlayer.LockedIn = true;
        currRoomPlayer.CharacterNum = characterNum;
        currRoomPlayer.CharacterWeaponSelection = characterWeaponSelection;
        
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
        // spawn empty gameobject to act as player gameobject (does not interact with game itself)
        GameObject player = Instantiate(emptyPlayerPrefab, Vector3.zero, Quaternion.identity);

        return player;
    }
    
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        int charactersToSpawn = roomPlayer.GetComponent<MyNetworkRoomPlayer>().CharacterNum;
        
        // Spawn all characters
        for (int i = 0; i < charactersToSpawn; i++)
        {
            Vector3 startPos = GetRandomStartPos(0, 50);
            GameObject character = Instantiate(playerPrefab, startPos, Quaternion.identity);
            AssignWeapons(character, roomPlayer, i);
            NetworkServer.Spawn(character, conn);

            _currSpawnedCharacterNum += 1;
        }

        // reset counters for next lobby
        if (_currSpawnedCharacterNum == _totalCharacterNum)
        {
            _currSpawnedCharacterNum = 0;
            _totalCharacterNum = 0;
            NetworkServer.SendToReady(new SetLocalCharactersMessage());
        }

        return true;
    }

    private Vector3 GetRandomStartPos(int minVal, int maxVal)
    {
        return new Vector3(Random.Range(minVal, maxVal), 0f, Random.Range(minVal, maxVal));
    }

    private void AssignWeapons(GameObject character, GameObject roomPlayer, int playerIndex)
    {
        PlayerWeapons playerWeapons = character.GetComponent<PlayerWeapons>();

        string[] weaponSelection = roomPlayer.GetComponent<MyNetworkRoomPlayer>().CharacterWeaponSelection[playerIndex];

        foreach (string weapon in weaponSelection)
            playerWeapons.WeaponsToAdd.Add(weapon);
    }
}
