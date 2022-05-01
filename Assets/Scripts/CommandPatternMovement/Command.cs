using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface ICommand
{
    event System.Action<ICommand> OnCompletion;
    void Execute();
}

public interface IQueueableCommand : ICommand
{
    string Name { get; }
}
