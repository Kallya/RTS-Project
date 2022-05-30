using UnityEngine;
using UnityEngine.UI;
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
    private Image _playerStateUIImg;
    [SerializeField] private RectTransform _playerStatePrefab;

    // each client sets their own name through the server
    // which is updated on other clients
    public override void OnStartLocalPlayer()
    {
        string playerName = MyNetworkManager.singleton.GetComponent<MainMenuConnect>().PlayerNameInput.text;

        MyNetworkManager netManager = MyNetworkManager.singleton as MyNetworkManager;
        foreach (MyNetworkRoomPlayer roomPlayer in netManager.roomSlots)
        {
            if (roomPlayer.PlayerName == playerName)
            {
                if (roomPlayer.isServer)
                    netManager.StopServer();
                else
                    netManager.StopClient();

                ErrorNotifier.Instance.GenerateErrorMsg(ErrorNotifier.ErrorMessages[ErrorMessageType.UnavailablePlayerName]);
            }
        }

        CmdSetPlayerName(playerName);
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        if (_playerStateUIImg == null)
            return;

        if (newReadyState == true)
        {
            uiTextRefs.PlayerStatus.text = "Ready";
            _playerStateUIImg.color = Color.green;
        }
        else
        {
            uiTextRefs.PlayerStatus.text = "Not Ready";
            _playerStateUIImg.color = Color.red;
        }

    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        PlayerName = playerName;
    }

    private void SetRoomPlayerUI(string oldPlayerName, string newPlayerName)
    {
        Transform playerStatePanel = GameObject.Find("LobbyGUI").transform.Find("Panel");

        _playerStateUI = Instantiate(_playerStatePrefab, playerStatePanel);
        
        _playerStateUIImg = _playerStateUI.GetComponent<Image>();
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
