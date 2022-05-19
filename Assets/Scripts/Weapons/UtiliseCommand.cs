using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// for using either a weapon or utility
public class UtiliseCommand : IQueueableCommand
{
    public string Name { get; } = "Use weapon";
    public event System.Action<IQueueableCommand> OnCompletion;

    private IEquipment _weapon;
    private uint _characterNetId;

    public UtiliseCommand(CharacterEquipment playerEquipment, uint netId)
    {
        _weapon = playerEquipment.ActiveEquipment;
        _characterNetId = netId;
    }

    public void Execute()
    {
        if (!CanActivate())
            return;

        if (_weapon is IWeapon weapon)
        {
            // only decrease energy if weapon actually fired
            // due to fixed attack rates
            if (weapon.Attack())
            {
                CharacterStatModifier.Instance.CmdDecreaseCharacterStat(_characterNetId, "Energy", _weapon.EnergyCost);
                PlayWeaponAudio(weapon);
            }
        }
        else if (_weapon is IUtility utility)
        {
            utility.Activate();
            PlayUtilityAudio(utility);
            CharacterStatModifier.Instance.CmdDecreaseCharacterStat(_characterNetId, "Energy", _weapon.EnergyCost);
        }

        OnCompletion?.Invoke(this);
    }

    private void PlayWeaponAudio(IWeapon weapon)
    {
        if (weapon is AssaultRifle)
            AudioManager.Instance.CmdPlayAudio(_characterNetId, "Assault Rifle Gunshot");
        else if (weapon is Shotgun)
            AudioManager.Instance.CmdPlayAudio(_characterNetId, "Shotgun Gunshot");
        else if (weapon is Sword)
            AudioManager.Instance.CmdPlayAudio(_characterNetId, "Sword Attack");
    }

    private void PlayUtilityAudio(IUtility utility)
    {
        if (utility is Barrier)
            AudioManager.Instance.CmdPlayAudio(_characterNetId, "Barrier Spawn");
        else if (utility is Wire)
            AudioManager.Instance.CmdPlayAudio(_characterNetId, "Wire Spawn");
    }

    private bool CanActivate()
    {
        return CharacterStatModifier.Instance.CanDecreaseStat(_characterNetId, "Energy", _weapon.EnergyCost);
    }
}