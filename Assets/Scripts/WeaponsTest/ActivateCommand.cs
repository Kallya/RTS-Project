using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ActivateCommand : IQueueableCommand
{
    public string Name { get; } = "Activate Utility";
    public event System.Action<ICommand> OnCompletion;

    private IUtility _utility;
    private uint _characterNetId;

    public ActivateCommand(GameObject character)
    {
        _utility = (IUtility)character.GetComponent<PlayerEquipment>().ActiveEquipment;
        _characterNetId = character.GetComponent<NetworkIdentity>().netId;
    }

    public void Execute()
    {
        if (CanActivate())
        {
            _utility.Activate();
            CharacterStatModifier.Instance.CmdDecreaseCharacterStat(_characterNetId, "Energy", _utility.EnergyCost);
        }

        OnCompletion?.Invoke(this);
    }

    private bool CanActivate()
    {
        return CharacterStatModifier.Instance.CanDecreaseStat(_characterNetId, "Energy", _utility.EnergyCost);
    }
}
