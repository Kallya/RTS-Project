using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MaintainCloakCommand : ICommand
{
    private static int s_cloakCost = 1;
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
            CharacterStatModifier.Instance.CmdDecreaseCharacterStat(_characterNetId, "Energy", s_cloakCost);
        else
            _characterCmdInput.ChangeCloak();

        _characterCmdInput.LastCloakCostTime = NetworkTime.time;
    }

    private bool CanCloak()
    {
        return CharacterStatModifier.Instance.CanDecreaseStat(_characterNetId, "Energy", s_cloakCost);
    }
}
