using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeaponCommand : ICommand
{
    public GameObject player { get; private set; }
    public event System.Action<ICommand> OnCompletion;

    private PlayerWeapons _playerWeapons;
    private int _weaponSlot;

    public SwitchWeaponCommand(GameObject player, int weaponSlot)
    {
        this.player = player;
        this._playerWeapons = player.GetComponent<PlayerWeapons>();
        this._weaponSlot = weaponSlot;
    }

    public void Execute()
    {
        _playerWeapons.SwitchWeapon(_weaponSlot);
        OnCompletion?.Invoke(this);
    }
}
