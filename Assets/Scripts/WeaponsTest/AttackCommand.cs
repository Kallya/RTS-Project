using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : ICommand
{
    public event System.Action<ICommand> OnCompletion;

    private IWeapon _weapon;

    public AttackCommand(GameObject player)
    {
        _weapon = (IWeapon)player.GetComponent<PlayerEquipment>().ActiveEquipment;
    }

    public void Execute()
    {
        _weapon.Attack();
        OnCompletion?.Invoke(this);
    }
}
