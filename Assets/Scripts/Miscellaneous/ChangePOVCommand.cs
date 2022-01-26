using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class ChangePOVCommand : ICommand
{
    public event System.Action<ICommand> OnCompletion;

    private int _nextCharacter;

    public ChangePOVCommand(GameObject player, int nextCharacter)
    {
        _nextCharacter = nextCharacter;
    }

    public void Execute()
    {
        POVManager.Instance.ChangePOV(_nextCharacter);    
        OnCompletion?.Invoke(this);
    }
}
