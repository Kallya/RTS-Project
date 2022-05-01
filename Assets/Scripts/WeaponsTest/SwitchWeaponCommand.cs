using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeaponCommand : IQueueableCommand
{
    public string Name { get; } = "Switch Weapon";
    public event System.Action<ICommand> OnCompletion;

    private PlayerEquipment _playerWeapons;
    private int _weaponSlot;

    public SwitchWeaponCommand(GameObject player, int weaponSlot)
    {
        _playerWeapons = player.GetComponent<PlayerEquipment>();
        _weaponSlot = weaponSlot;
    }

    public void Execute()
    {
        _playerWeapons.CmdSwitchEquipment(_weaponSlot);

        OnCompletion?.Invoke(this);
    }
}
