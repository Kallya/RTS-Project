using UnityEngine;
using Mirror;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    public bool LockedIn { get; set; } = false;
}
