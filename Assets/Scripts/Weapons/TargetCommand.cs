using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetCommand : ICommand
{
    private GameObject _character;
    private NavMeshAgent _characterNavMeshAgent;
    private Transform _target;
    private CommandProcessor _characterCmdProcessor;
    private float _equipRange = 2f;
    private static float _rangeAllowance = 0.5f; // by how much within the equip range the character stops to account for acceleration time

    public TargetCommand(GameObject character, Transform target, CharacterEquipment characterEquipment, CommandProcessor characterCmdProcessor)
    {
        _character = character;
        _target = target;
        _characterCmdProcessor = characterCmdProcessor;

        if (characterEquipment.ActiveEquipment is IWeapon weapon)
            _equipRange = weapon.Range;

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
        // stop when in offset range
        if (Vector3.Distance(_character.transform.position, _target.position) <= _equipRange-_rangeAllowance)
            _characterNavMeshAgent.isStopped = true;
        else
            _characterNavMeshAgent.isStopped = false; // if isStopped was set to true after reaching target, this will start movement again

        _characterCmdProcessor.ExecuteCommand(new MoveCommand(_character, _target.position));
    }
}
