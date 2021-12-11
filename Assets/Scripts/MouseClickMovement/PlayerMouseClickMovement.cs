using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerMouseInput))]
public class PlayerMouseClickMovement : MonoBehaviour
{
    private PlayerMouseInput _playerMouseInput;
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _playerMouseInput = GetComponent<PlayerMouseInput>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        MoveToNextPosition();
    }

    private void MoveToNextPosition()
    {
        _navMeshAgent.destination = _playerMouseInput.NextPosition;
    }
}
