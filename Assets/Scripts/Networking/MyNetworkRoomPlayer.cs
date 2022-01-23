using UnityEngine;
using Mirror;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    public bool LockedIn { get; set; } = false;
    public int CharacterNum { get; set; } = 1;
    public string[][] CharacterWeaponSelection { get; set; }
}
