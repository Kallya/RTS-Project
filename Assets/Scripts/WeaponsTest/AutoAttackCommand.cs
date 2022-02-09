using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttackCommand : ICommand
{
    public event System.Action<ICommand> OnCompletion;

    private IWeapon _weapon;
    private Transform _playerTransform;

    public AutoAttackCommand(GameObject player)
    {
        _weapon = (IWeapon)player.GetComponent<PlayerEquipment>().ActiveEquipment;
        _playerTransform = player.transform;
    }

    public void Execute()
    {
        if (_weapon == null)
        {
            OnCompletion?.Invoke(this);
            return;
        }

        Collider[] collInRange = Physics.OverlapSphere(_playerTransform.position, _weapon.Range);
        
        if (collInRange.Length != 0)
        {
            foreach (Collider coll in collInRange)
            {
                Vector3 collDir = coll.transform.position - _playerTransform.position;

                // enemy and in front of character
                if (coll.tag == "Enemy" && Vector3.Dot(_playerTransform.forward, collDir) > 0)
                {
                    //Vector3 newDir = Vector3.RotateTowards(_playerTransform.forward, collDir, (Mathf.PI/2) * Time.deltaTime, 0f);
                    //_playerTransform.rotation = Quaternion.LookRotation(newDir);
                    _playerTransform.LookAt(coll.transform);
                    _weapon.Attack();
                    break;
                }
            }
        }

        OnCompletion?.Invoke(this);
    }
}
