using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommand : IQueueableCommand
{
    public string Name { get; } = "Move";
    public event System.Action<IQueueableCommand> OnCompletion;

    private NavMeshAgent _playerNavMeshAgent;
    private Vector3 _destination;
    private MoveCommandCompletionObserver _completionObserver;
    private LineRenderer _movePathLine;

    public MoveCommand(GameObject player, Vector3 destination)
    {
        _playerNavMeshAgent = player.GetComponent<NavMeshAgent>();
        _completionObserver = player.GetComponent<MoveCommandCompletionObserver>();
        _movePathLine = player.GetComponent<LineRenderer>();
        _destination = destination;
    }

    public void Execute()
    {
        MoveToNextPosition();
        _completionObserver.enabled = true;
        _completionObserver.OnDestinationReached += DestinationReached;
    }

    private void MoveToNextPosition()
    {
        NavMeshPath path = new NavMeshPath();
        _playerNavMeshAgent.CalculatePath(_destination, path); // calculate path in advance so full path is available to draw
        _playerNavMeshAgent.SetDestination(_destination);

        DrawPath(path);
    }

    private void DestinationReached()
    {
        _completionObserver.enabled = false;
        _completionObserver.OnDestinationReached -= DestinationReached;

        _movePathLine.positionCount = 0; // remove path indicator

        OnCompletion?.Invoke(this);
    }

    private void OnDestroy()
    {
        // stop character movement if undone during its execution
        if (_completionObserver.enabled == false)
            _playerNavMeshAgent.SetDestination(_playerNavMeshAgent.transform.position);
    }

    // draw character's movement path
    private void DrawPath(NavMeshPath path)
    {
        // if character's destination is its current pos or null
        // path doesn't need to be drawn
        if (path.corners.Length < 2)
            return;

        _movePathLine.positionCount = path.corners.Length;

        _movePathLine.SetPositions(path.corners); // set all vertices in path
    }
}
