using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerConnect : NetworkBehaviour
{
    // using start because if using OnStartLocalPlayer
    // for some reason other characters aren't instantiated yet
    private void Start()
    {
        if (!isLocalPlayer)
            return;
        
        POVManager.Instance.SetLocalCharacters();
    }
}
