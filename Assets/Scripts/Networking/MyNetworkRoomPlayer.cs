using UnityEngine;
using Mirror;
using TMPro;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    public bool LockedIn { get; set; } = false;
    public int CharacterNum { get; set; } = 1;
    public string PlayerName { get; set; }
    public string[][] CharacterWeaponSelection { get; set; }

    public override void OnGUI()
    {
        MyNetworkManager room = MyNetworkManager.singleton as MyNetworkManager;

        RectTransform playerStateUI = Instantiate(room.PlayerStatePrefab, room.PlayerStatePanel);
        playerStateUI.offsetMin = new Vector2(0f, (index-1) * 263f);
        playerStateUI.offsetMax = new Vector2(0f, index * 263f);
        
        LobbyPlayerTextRefs textRefs = playerStateUI.GetComponent<LobbyPlayerTextRefs>();
        textRefs.PlayerName.text = PlayerName;

        if (readyToBegin == true)
            textRefs.PlayerStatus.text = "Ready";
        else
            textRefs.PlayerStatus.text = "Not Ready";
    }
}
