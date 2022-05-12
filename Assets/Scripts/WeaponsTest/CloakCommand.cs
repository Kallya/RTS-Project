using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CloakCommand : IQueueableCommand
{
    public string Name { get; } = "Cloak Character";
    public event System.Action<IQueueableCommand> OnCompletion;

    private static int _cloakCost = 1; // energy lost per interval
    private uint _characterNetId;
    private bool _isCloaked;

    public CloakCommand(bool isCloaked, uint netId)
    {
        _characterNetId = netId;
        _isCloaked = isCloaked;
    }

    public void Execute()
    {
        CloakMessage msg = new CloakMessage() {
            CharacterNetId=_characterNetId,
            IsCloaked=_isCloaked
        };

        NetworkClient.Send(msg);
        
        OnCompletion?.Invoke(this);
    }
}
