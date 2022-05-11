using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface ICommand
{
    void Execute();
}

public interface IQueueableCommand : ICommand
{
    event System.Action<IQueueableCommand> OnCompletion;
    string Name { get; }
}
