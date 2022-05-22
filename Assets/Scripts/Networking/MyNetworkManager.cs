using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// empty message to trigger UI enable for character setup
public struct StartPreGameMessage : NetworkMessage {}
// empty message to tell clients when to call SetLocalCharacters
// to ensure all characters (allied and enemy) are setup
public struct SetLocalCharactersMessage : NetworkMessage {}
public struct FinishGameMessage : NetworkMessage
{
    public Score[] FinalScores;
    public int[] WinnerIds;
}

public struct SetScoreboardMessage : NetworkMessage
{
    public int[] ConnectionIds;
    public string[] PlayerNames;
    public int[] TeamSizes;
}

public class MyNetworkManager : NetworkRoomManager
{  
    public List<GameObject> SpawnedCharacters = new List<GameObject>();

    [SerializeField] private GameObject _emptyPlayerPrefab;
    [SerializeField] private GameObject _kabukiCharacterPrefab;
    [SerializeField] private GameObject _tenguCharacterPrefab;
    [SerializeField] private GameObject _kitsuneCharacterPrefab;
    private int _totalCharacterNum = 0;
    private int _currSpawnedCharacterNum = 0;

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        if (sceneName == RoomScene)
            NetworkServer.RegisterHandler<CharacterConfigurationMessage>(OnNetworkLockIn);
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
        CommandsDisplayHUDSetup.Instance.Setup();

        CharacterCommandInput.InputsEnabled = true; // enable all inputs when everything's loaded
        Clock.Instance.IsTicking = true; // start clock when everything's loaded
        AudioManager.Instance.ToggleGameTrack(true); // start background music
    }

    private void OnSetScoreboardMessage(SetScoreboardMessage msg)
    {
        ScoreManager.Instance.gameObject.SetActive(false);
        ScoreManager.Instance.SetScoreboard(msg.ConnectionIds, msg.PlayerNames, msg.TeamSizes);
        NetworkClient.UnregisterHandler<SetScoreboardMessage>();

        // setup for endgame triggered in scoreboardmanager
        EndgameManager.Instance.gameObject.SetActive(false);
        NetworkClient.RegisterHandler<FinishGameMessage>(OnFinishGame);
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
        NetworkServer.UnregisterHandler<CharacterConfigurationMessage>();

        ServerChangeScene(GameplayScene);
    }

    private void OnFinishGame(FinishGameMessage msg)
    {
        NetworkClient.UnregisterHandler<FinishGameMessage>();

        CharacterCommandInput.InputsEnabled = false; // disable all inputs (all clients)
        Clock.Instance.IsTicking = false; // stop clock
        AudioManager.Instance.ToggleGameTrack(false); // stop music

        EndgameManager.Instance.Setup(msg.FinalScores, msg.WinnerIds); // generate final scoreboard + outcome of game
    }

    public void OnNetworkLockIn(NetworkConnection conn, CharacterConfigurationMessage msg)
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
        currRoomPlayer.CharacterTypes = msg.CharacterTypes;
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

    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        GameObject roomPlayer = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);

        return roomPlayer;
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        // spawn empty gameobject to act as player gameobject (does not interact with game itself)
        GameObject player = Instantiate(_emptyPlayerPrefab, Vector3.zero, Quaternion.identity);

        return player;
    }
    
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        int charactersToSpawn = roomPlayer.GetComponent<MyNetworkRoomPlayer>().CharacterNum;
        
        // Spawn all characters
        for (int i = 0; i < charactersToSpawn; i++)
        {
            Vector3 startPos = GetRandomStartPos(0, 50);

            string characterType = roomPlayer.GetComponent<MyNetworkRoomPlayer>().CharacterTypes[i];
            GameObject character = null;
            
            switch (characterType)
            {
                case "Kabuki (Tank)":
                    character = Instantiate(_kabukiCharacterPrefab, startPos, Quaternion.identity);
                    break;
                case "Tengu (All-Rounder)":
                    character = Instantiate(_tenguCharacterPrefab, startPos, Quaternion.identity);
                    break;
                case "Kitsune (Assassin)":
                    character = Instantiate(_kitsuneCharacterPrefab, startPos, Quaternion.identity);
                    break;
            }

            AssignWeapons(character, roomPlayer, i);
            NetworkServer.Spawn(character, conn);
            SpawnedCharacters.Add(character);

            _currSpawnedCharacterNum += 1;
        }

        // when all characters spawned
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

            SetScoreboardMessage scoreboardMsg = new SetScoreboardMessage()
            {
                ConnectionIds=connectionIds.ToArray(),
                PlayerNames=playerNames.ToArray(),
                TeamSizes=teamSizes.ToArray()
            };

            NetworkServer.SendToReady(scoreboardMsg);

            // setup server managers for game elements
            CharacterStatModifier.Instance.Setup();
            AudioManager.Instance.Setup();
        }

        return true;
    }

    private Vector3 GetRandomStartPos(int minVal, int maxVal)
    {
        return new Vector3(Random.Range(minVal, maxVal), 0f, Random.Range(minVal, maxVal));
    }

    private void AssignWeapons(GameObject character, GameObject roomPlayer, int playerIndex)
    {
        CharacterEquipment playerWeapons = character.GetComponent<CharacterEquipment>();

        string[] weaponSelection = roomPlayer.GetComponent<MyNetworkRoomPlayer>().CharacterWeaponSelection[playerIndex];

        foreach (string weapon in weaponSelection)
            playerWeapons.EquipmentToAdd.Add(weapon);
    }
}
