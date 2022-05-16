using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CloakCommand : IQueueableCommand
{
    public string Name { get; } = "Cloak Character";
    public event System.Action<IQueueableCommand> OnCompletion;
    
    private uint _characterNetId;
    private bool _isCloaked;
    private CharacterCommandInput _characterCmdInput;

    public CloakCommand(CharacterCommandInput characterCmdInput, bool isCloaked, uint netId)
    {
        _characterNetId = netId;
        _isCloaked = isCloaked;
        _characterCmdInput = characterCmdInput;
    }

    public void Execute()
    {
        CloakMessage msg = new CloakMessage() {
            CharacterNetId=_characterNetId,
            IsCloaked=_isCloaked
        };

        NetworkClient.Send(msg);
        _characterCmdInput.LastCloakCostTime = NetworkTime.time;
        
        OnCompletion?.Invoke(this);
    }
}
