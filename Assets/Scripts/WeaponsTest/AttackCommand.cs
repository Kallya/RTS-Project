using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : ICommand
{
    public GameObject player { get; private set; }
    public event System.Action<ICommand> OnCompletion;

    private IWeapon _weapon;

    public AttackCommand(GameObject player)
    {
        this.player = player;

        this._weapon = (IWeapon)player.GetComponent<PlayerWeapons>().ActiveEquipment;
    }

    public void Execute()
    {
        _weapon?.Attack();
        OnCompletion?.Invoke(this);
    }
}
