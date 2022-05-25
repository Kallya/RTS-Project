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
        _movePathLine.SetPosition(0, _playerNavMeshAgent.transform.position);

        _playerNavMeshAgent.SetDestination(_destination);
        DrawPath(_playerNavMeshAgent.path);
    }

    private void DestinationReached()
    {
        _completionObserver.enabled = false;
        _completionObserver.OnDestinationReached -= DestinationReached;

        OnCompletion?.Invoke(this);
    }

    private void OnDestroy()
    {
        // stop character movement if undone during its execution
        if (_completionObserver.enabled == false)
            _playerNavMeshAgent.SetDestination(_playerNavMeshAgent.transform.position);
    }

    // draw character's movement path
    private IEnumerator DrawPath(NavMeshPath path)
    {
        yield return new WaitForEndOfFrame();

        // if character's destination is its current pos or null
        // path doesn't need to be drawn
        if (path.corners.Length < 2)
            yield return null;

        _movePathLine.positionCount = path.corners.Length;

        _movePathLine.SetPositions(path.corners); // set all vertices in path

        yield return null;
    }
}
