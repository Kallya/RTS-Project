using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class ChangePOVCommand : ICommand
{
    public event System.Action<ICommand> OnCompletion;

    private int _nextCharacter;
    private PlayerCommandInput _playerInput;

    public ChangePOVCommand(GameObject player, int nextCharacter)
    {
        _nextCharacter = nextCharacter;
        _playerInput = player.GetComponent<PlayerCommandInput>();
    }

    public void Execute()
    {
        _playerInput.enabled = false;
        POVManager.Instance.ChangePOV(_nextCharacter);
        OnCompletion?.Invoke(this);
    }
}
