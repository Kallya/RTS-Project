using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// empty message to trigger UI enable for character setup
public struct StartPreGameMessage : NetworkMessage {}
// empty message to tell clients when to call SetLocalCharacters
// to ensure all characters (allied and enemy) are setup
public struct SetLocalCharactersMessage : NetworkMessage {}
public struct SetScoreboardMessage : NetworkMessage
{
    public int[] ConnectionIds;
    public string[] PlayerNames;
    public int[] TeamSizes;
}

public class MyNetworkManager : NetworkRoomManager
{  
    public RectTransform PlayerStatePrefab;

    [SerializeField] private GameObject _emptyPlayerPrefab;
    private int _totalCharacterNum = 0;
    private int _currSpawnedCharacterNum = 0;

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        if (sceneName == RoomScene)
        {
            NetworkServer.RegisterHandler<WeaponSelectionMessage>(OnNetworkLockIn);

            // disable UI because it's automatically enabled on spawn
            // need to change cause no longer need to ref eventsystem
        }   
    }

    public override void OnRoomStartClient()
    {
        NetworkClient.RegisterHandler<StartPreGameMessage>(OnStartPreGame);
        NetworkClient.RegisterHandler<SetLocalCharactersMessage>(OnSetLocalCharacters);
        NetworkClient.RegisterHandler<SetScoreboardMessage>(OnSetScoreboardMessage);
    }


    // synchronise character setup start on clients (not just server)
    private void OnStartPreGame(StartPreGameMessage msg)
    {
        UIObjectReferences.Instance.CharacterSetupUI.SetActive(true);
    }

    private void OnSetLocalCharacters(SetLocalCharactersMessage msg)
    {
        POVManager.Instance.SetLocalCharacters();
        NetworkClient.UnregisterHandler<SetLocalCharactersMessage>();

        TeamMiniHUDSetup.Instance.Setup();
    }

    private void OnSetScoreboardMessage(SetScoreboardMessage msg)
    {
        ScoreManager.Instance.gameObject.SetActive(false);
        ScoreManager.Instance.SetScoreboard(msg.ConnectionIds, msg.PlayerNames, msg.TeamSizes);
        NetworkClient.UnregisterHandler<SetScoreboardMessage>();
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

        // set customisation for player's team
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

    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn)
    {
        GameObject roomPlayer = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);

        return roomPlayer;
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        // spawn empty gameobject to act as player gameobject (does not interact with game itself)
        GameObject player = Instantiate(_emptyPlayerPrefab, Vector3.zero, Quaternion.identity);

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
            character.name = conn.connectionId.ToString();
            AssignWeapons(character, roomPlayer, i);
            NetworkServer.Spawn(character, conn);

            _currSpawnedCharacterNum += 1;
        }

        if (_currSpawnedCharacterNum == _totalCharacterNum)
        {
            // reset counters for next lobby
            _currSpawnedCharacterNum = 0;
            _totalCharacterNum = 0;

            NetworkServer.SendToReady(new SetLocalCharactersMessage()); // tell clients to assign setup relative allied and enemy characters

            // send info for scoreboard setup
            List<int> connectionIds = new List<int>();
            List<string> playerNames = new List<string>();
            List<int> teamSizes = new List<int>();

            foreach(MyNetworkRoomPlayer player in roomSlots)
            {
                connectionIds.Add(player.connectionToClient.connectionId);
                playerNames.Add(player.PlayerName);
                teamSizes.Add(player.CharacterNum);
            }

            SetScoreboardMessage msg = new SetScoreboardMessage()
            {
                ConnectionIds=connectionIds.ToArray(),
                PlayerNames=playerNames.ToArray(),
                TeamSizes=teamSizes.ToArray()
            };
            NetworkServer.SendToReady(msg);
        }

        return true;
    }

    private Vector3 GetRandomStartPos(int minVal, int maxVal)
    {
        return new Vector3(Random.Range(minVal, maxVal), 0f, Random.Range(minVal, maxVal));
    }

    private void AssignWeapons(GameObject character, GameObject roomPlayer, int playerIndex)
    {
        PlayerEquipment playerWeapons = character.GetComponent<PlayerEquipment>();

        string[] weaponSelection = roomPlayer.GetComponent<MyNetworkRoomPlayer>().CharacterWeaponSelection[playerIndex];

        foreach (string weapon in weaponSelection)
            playerWeapons.EquipmentToAdd.Add(weapon);
    }
}
