using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AttackCommand : IQueueableCommand
{
    public string Name { get; } = "Attack";
    public event System.Action<ICommand> OnCompletion;

    private GameObject _character;
    private IWeapon _weapon;
    private uint _characterNetId;

    public AttackCommand(GameObject character)
    {
        _character = character;
        _weapon = (IWeapon)character.GetComponent<PlayerEquipment>().ActiveEquipment;
        _characterNetId = character.GetComponent<NetworkIdentity>().netId;
    }

    public void Execute()
    {
        if (CanAttack())
        {
            if (_weapon.Attack())
                CharacterStatModifier.Instance.CmdDecreaseCharacterStat(_characterNetId, "Energy", _weapon.EnergyCost);
        }
        else
            Debug.Log("not enough energy");
            
        OnCompletion?.Invoke(this);
    }


    private bool CanAttack()
    {
        return CharacterStatModifier.Instance.CanDecreaseStat(_characterNetId, "Energy", _weapon.EnergyCost);
    }

}
