using UnityEngine;
using Mirror;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    public bool LockedIn { get; set; } = false;
    public int CharacterNum { get; set; } = 1;
    public string[] CharacterTypes { get; set; }
    public string[][] CharacterWeaponSelection { get; set; }
    [SyncVar(hook=nameof(SetRoomPlayerUI))] public string PlayerName;
    public LobbyPlayerTextRefs uiTextRefs { get; private set; }

    private RectTransform _playerStateUI;

    public override void OnStartLocalPlayer()
    {
        string playerName = MyNetworkManager.singleton.GetComponent<MainMenuConnect>().PlayerNameInput.text;

        CmdSetPlayerName(playerName);
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        if (newReadyState == true)
            uiTextRefs.PlayerStatus.text = "Ready";
        else
            uiTextRefs.PlayerStatus.text = "Not Ready";

    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        PlayerName = playerName;
    }

    private void SetRoomPlayerUI(string oldPlayerName, string newPlayerName)
    {
        if (newPlayerName == null)
            return; 

        Transform playerStatePanel = GameObject.Find("LobbyGUI").transform.Find("Panel");
        MyNetworkManager room = MyNetworkManager.singleton as MyNetworkManager;

        _playerStateUI = Instantiate(room.PlayerStatePrefab, playerStatePanel);
        _playerStateUI.offsetMin = new Vector2(0f, 510 - index*170);
        _playerStateUI.offsetMax = new Vector2(0f, -index*170);
        
        uiTextRefs = _playerStateUI.GetComponent<LobbyPlayerTextRefs>();
        uiTextRefs.PlayerName.text = PlayerName;
    }

    private void OnDestroy()
    {
        if (_playerStateUI == null)
            return;
            
        Destroy(_playerStateUI.gameObject);
    }
}
