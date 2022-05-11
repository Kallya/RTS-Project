using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// for using either a weapon or utility
public class UtiliseCommand : IQueueableCommand
{
    public string Name { get; } = "Use Equipment";
    public event System.Action<IQueueableCommand> OnCompletion;

    private IEquipment _equipment;
    private uint _characterNetId;

    public UtiliseCommand(CharacterEquipment playerEquipment, uint netId)
    {
        _equipment = playerEquipment.ActiveEquipment;
        _characterNetId = netId;
    }

    public void Execute()
    {
        if (CanActivate())
        {
            if (_equipment is IWeapon weapon)
            {
                // only decrease energy if weapon actually fired
                // due to fixed fire rates
                if (weapon.Attack())
                    CharacterStatModifier.Instance.CmdDecreaseCharacterStat(_characterNetId, "Energy", _equipment.EnergyCost);
            }
            else if (_equipment is IUtility utility)
            {
                utility.Activate();
                CharacterStatModifier.Instance.CmdDecreaseCharacterStat(_characterNetId, "Energy", _equipment.EnergyCost);
            }
        }

        OnCompletion?.Invoke(this);
    }

    private bool CanActivate()
    {
        return CharacterStatModifier.Instance.CanDecreaseStat(_characterNetId, "Energy", _equipment.EnergyCost);
    }
}