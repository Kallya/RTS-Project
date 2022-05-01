using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manage the completion of all in game actions from user input
public class CommandProcessor : MonoBehaviour
{
    public event System.Action<IQueueableCommand> OnCommandQueued;
    public event System.Action OnCommandDequeued;
    // private List<ICommand> _commands = new List<ICommand>();
    private List<IQueueableCommand> _queuedCommands = new List<IQueueableCommand>();

    public void ExecuteCommand(ICommand command)
    {
        // _commands.Add(command);
        command.Execute();
    }

    public void QueueCommand(IQueueableCommand command)
    {
        _queuedCommands.Add(command);
        OnCommandQueued?.Invoke(command);

        if (_queuedCommands.Count == 1)
            ExecuteNextCommand();
    }

    public void Undo()
    {
        if (_queuedCommands.Count == 0) 
            return;

        _queuedCommands.RemoveAt(_queuedCommands.Count - 1);
        OnCommandDequeued?.Invoke();
    }

    private void Completion(ICommand command)
    {
        command.OnCompletion -= Completion;
        _queuedCommands.RemoveAt(0);
        OnCommandDequeued?.Invoke();

        ExecuteNextCommand();
    }

    private void ExecuteNextCommand()
    {
        if (_queuedCommands.Count == 0)
            return;

        IQueueableCommand nextCommand = _queuedCommands[0];
        nextCommand.OnCompletion += Completion;
        nextCommand.Execute();
    }

}
