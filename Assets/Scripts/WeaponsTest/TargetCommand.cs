using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetCommand : ICommand
{
    private GameObject _character;
    private float _weaponRange = 2f;
    private NavMeshAgent _characterNavMeshAgent;
    private Transform _target;

    public TargetCommand(GameObject character, Transform target, CharacterEquipment characterEquipment)
    {
        _character = character;
        _target = target;

        if (characterEquipment.ActiveEquipment is IWeapon weapon)
            _weaponRange = weapon.Range;

        _characterNavMeshAgent = character.GetComponent<NavMeshAgent>();
    }

// how to switch off target?
    public void Execute()
    {
        if (_target != null)
            MoveToNextPosition();
    }

    private void MoveToNextPosition()
    {
        if (Vector3.Distance(_character.transform.position, _target.position) <= _weaponRange-0.5)
            _characterNavMeshAgent.isStopped = true;
        else
            _characterNavMeshAgent.isStopped = false;

        _characterNavMeshAgent.destination = _target.position;
    }
}
