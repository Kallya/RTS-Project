using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeaponCommand : IQueueableCommand
{
    public string Name { get; } = "Switch Weapon";
    public event System.Action<IQueueableCommand> OnCompletion;

    private CharacterEquipment _characterEquipment;
    private int _weaponSlot;

    public SwitchWeaponCommand(CharacterEquipment characterEquipment, int weaponSlot)
    {
        _characterEquipment = characterEquipment;
        _weaponSlot = weaponSlot;
    }

    public void Execute()
    {
        _characterEquipment.CmdSwitchEquipment(_weaponSlot);

        OnCompletion?.Invoke(this);
    }
}
