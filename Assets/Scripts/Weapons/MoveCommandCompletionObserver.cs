using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommandCompletionObserver : MonoBehaviour
{
    public event System.Action OnDestinationReached;

    private NavMeshAgent _navMeshAgent;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance || _navMeshAgent.isStopped == true)
            OnDestinationReached?.Invoke();
    }
}
