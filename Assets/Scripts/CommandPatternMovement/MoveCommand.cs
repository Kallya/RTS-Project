using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommand : ICommand
{
    // this player property is currently redundant
    public GameObject player { get; private set; }
    public event System.Action<ICommand> OnCompletion;

    private NavMeshAgent _playerNavMeshAgent;
    private Vector3 _destination;
    private MoveCommandCompletionObserver _completionObserver;

    public MoveCommand(GameObject player, Vector3 destination)
    {
        this.player = player;

        this._playerNavMeshAgent = player.GetComponent<NavMeshAgent>();
        this._destination = destination;
        this._completionObserver = player.GetComponent<MoveCommandCompletionObserver>();
    }

    public void Execute()
    {
        MoveToNextPosition();
        _completionObserver.enabled = true;
        _completionObserver.OnDestinationReached += DestinationReached;
    }

    private void MoveToNextPosition()
    {
        _playerNavMeshAgent.destination = _destination;
    }

    private void DestinationReached()
    {
        _completionObserver.enabled = false;
        _completionObserver.OnDestinationReached -= DestinationReached;
        OnCompletion?.Invoke(this);
    }
}
