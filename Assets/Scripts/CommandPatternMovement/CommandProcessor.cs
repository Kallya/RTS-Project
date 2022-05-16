using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manage the completion of all in game actions from user input
public class CommandProcessor : MonoBehaviour
{
    public event System.Action<IQueueableCommand> OnCommandQueued;
    public event System.Action<IQueueableCommand> OnQueueableCommandExecuted;
    public event System.Action OnCommandCompleted;
    public event System.Action OnCommandUndone;
    // private List<ICommand> _commands = new List<ICommand>();
    private List<IQueueableCommand> _queuedCommands = new List<IQueueableCommand>();

    public void ExecuteCommand(ICommand command)
    {
        if (command is IQueueableCommand queueableCommand)
            OnQueueableCommandExecuted?.Invoke(queueableCommand);

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
        OnCommandUndone?.Invoke();
    }

    private void Completion(IQueueableCommand command)
    {
        command.OnCompletion -= Completion;

        if (_queuedCommands.Count != 0)
        {
            _queuedCommands.RemoveAt(0);
            OnCommandCompleted?.Invoke(); // command only completed if it wasn't undone
        }

        ExecuteNextCommand();
    }

    private void ExecuteNextCommand()
    {
        if (_queuedCommands.Count == 0)
            return;

        IQueueableCommand nextCommand = _queuedCommands[0];
        nextCommand.OnCompletion += Completion;
        ExecuteCommand(nextCommand);
    }

}
