using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainCloakCommand : ICommand
{
    private static int _cloakCost = 1;
    private uint _characterNetId;
    private CharacterCommandInput _characterCmdInput;

    public MaintainCloakCommand(CharacterCommandInput characterCmdInput, uint characterNetId)
    {
        _characterNetId = characterNetId;
        _characterCmdInput = characterCmdInput;
    }

    public void Execute()
    {
        if (CanCloak())
            CharacterStatModifier.Instance.CmdDecreaseCharacterStat(_characterNetId, "Energy", _cloakCost);
        else
            _characterCmdInput.ChangeCloak();
    }

    private bool CanCloak()
    {
        return CharacterStatModifier.Instance.CanDecreaseStat(_characterNetId, "Energy", _cloakCost);
    }
}
