using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePOVCommand : ICommand
{
    private int _nextCharacter;

    public ChangePOVCommand(int nextCharacter)
    {
        _nextCharacter = nextCharacter;
    }

    public void Execute()
    {
        POVManager.Instance.ChangePOV(_nextCharacter);
    }
}
