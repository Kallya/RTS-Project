using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetCommand : ICommand
{
    public event System.Action<ICommand> OnCompletion;

    private GameObject _player;
    private float _weaponRange = 2f;
    private NavMeshAgent _playerNavMeshAgent;
    private Transform _target;

    public TargetCommand(GameObject player, Transform target)
    {
        _player = player;
        _target = target;

        IEquipment playerEquip = player.GetComponent<PlayerEquipment>().ActiveEquipment;
        if (playerEquip is IWeapon weapon)
            _weaponRange = weapon.Range;

        _playerNavMeshAgent = player.GetComponent<NavMeshAgent>();
    }

// how to switch off target?
    public void Execute()
    {
        if (_target != null)
            MoveToNextPosition();

        OnCompletion?.Invoke(this);
    }

    private void MoveToNextPosition()
    {
        if (Vector3.Distance(_player.transform.position, _target.position) <= _weaponRange-0.5)
            _playerNavMeshAgent.isStopped = true;
        else
            _playerNavMeshAgent.isStopped = false;

        _playerNavMeshAgent.destination = _target.position;
    }
}
