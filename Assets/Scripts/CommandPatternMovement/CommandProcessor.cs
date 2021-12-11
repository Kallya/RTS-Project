using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandProcessor : MonoBehaviour
{
    private List<ICommand> _commands = new List<ICommand>();
    private List<ICommand> _queuedCommands = new List<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        _commands.Add(command);
        command.Execute();
    }

    public void QueueCommand(ICommand command)
    {
        _queuedCommands.Add(command);

        if (_queuedCommands.Count == 1)
            ExecuteNextCommand();
    }

    public void Undo()
    {
        if (_queuedCommands.Count == 0) 
            return;

        _queuedCommands.RemoveAt(_queuedCommands.Count - 1);
    }

    private void Completion(ICommand command)
    {
        command.OnCompletion -= Completion;
        _queuedCommands.RemoveAt(0);

        ExecuteNextCommand();
    }

    private void ExecuteNextCommand()
    {
        if (_queuedCommands.Count == 0)
            return;

        ICommand nextCommand = _queuedCommands[0];
        nextCommand.OnCompletion += Completion;
        nextCommand.Execute();
    }

}
