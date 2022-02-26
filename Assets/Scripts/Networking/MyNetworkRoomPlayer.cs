using UnityEngine;
using Mirror;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    public bool LockedIn { get; set; } = false;
    public int CharacterNum { get; set; } = 1;
    public string[][] CharacterWeaponSelection { get; set; }
    [SyncVar] public string PlayerName;

    public override void OnStartLocalPlayer()
    {
        string playerName = MyNetworkManager.singleton.GetComponent<MainMenuConnect>().PlayerNameInput.text;

        CmdSetPlayerName(playerName);
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        PlayerName = playerName;
    }

    public override void OnClientEnterRoom()
    {
        SetRoomPlayerUI();
    }

    private void SetRoomPlayerUI()
    {
        MyNetworkManager room = MyNetworkManager.singleton as MyNetworkManager;

        RectTransform playerStateUI = Instantiate(room.PlayerStatePrefab, room.PlayerStatePanel);
        playerStateUI.offsetMin = new Vector2(0f, 510 - index*170);
        playerStateUI.offsetMax = new Vector2(0f, -index*170);
        
        LobbyPlayerTextRefs textRefs = playerStateUI.GetComponent<LobbyPlayerTextRefs>();
        textRefs.PlayerName.text = PlayerName;
    }

}
