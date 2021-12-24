using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerConnect : NetworkBehaviour
{
    private void Start()
    {
        POVManager.Instance.SetLocalCharacters();
    }
}
