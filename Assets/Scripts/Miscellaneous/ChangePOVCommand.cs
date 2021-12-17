using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePOVCommand : ICommand
{
    public event System.Action<ICommand> OnCompletion;

    private int _nextCharacterIndex;
    private PlayerCommandInput _playerInput;

    public ChangePOVCommand(GameObject player, int nextCharacterIndex)
    {
        this._nextCharacterIndex = nextCharacterIndex;
        this._playerInput = player.GetComponent<PlayerCommandInput>();
    }

    public void Execute()
    {
        _playerInput.enabled = false;
        ChangePOVListener.Instance.ChangePOV(_nextCharacterIndex);
        OnCompletion?.Invoke(this);
    }
}
