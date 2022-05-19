using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttackCommand : ICommand
{
    private IWeapon _weapon;
    private Transform _playerTransform;
    private uint _characterNetId;

    public AutoAttackCommand(GameObject player, uint characterNetId)
    {
        _weapon = (IWeapon)player.GetComponent<CharacterEquipment>().ActiveEquipment;
        _characterNetId = characterNetId;
        _playerTransform = player.transform;
    }

    public void Execute()
    {
        if (_weapon == null)
            return;
        
        if (!CanAttack())
            return;

        Collider[] collInRange = Physics.OverlapSphere(_playerTransform.position, _weapon.Range);
        
        if (collInRange.Length == 0)
            return;

        // check and shoot if enemies in front
        foreach (Collider coll in collInRange)
        {
            Vector3 collDir = coll.transform.position - _playerTransform.position;

            // enemy and in front of character
            if (coll.tag == "Enemy" && Vector3.Dot(_playerTransform.forward, collDir) > 0)
            {
                _playerTransform.LookAt(coll.transform);

                if (_weapon.Attack())
                    CharacterStatModifier.Instance.CmdDecreaseCharacterStat(_characterNetId, "Energy", _weapon.EnergyCost);

                break;
            }
        }
    }

    private bool CanAttack()
    {
        return CharacterStatModifier.Instance.CanDecreaseStat(_characterNetId, "Energy", _weapon.EnergyCost);
    }
}
