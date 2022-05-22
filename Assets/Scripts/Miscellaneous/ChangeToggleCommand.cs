using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeToggleCommand : IQueueableCommand
{
    public string Name { get; private set; } = "Toggle";
    public event System.Action<IQueueableCommand> OnCompletion;
    public string ToggleName { get; private set; }

    private CharacterCommandInput _characterInput;

    public ChangeToggleCommand(CharacterCommandInput characterInput, string toggleName)
    {
        _characterInput = characterInput;
        ToggleName = toggleName;

        // add toggle name to identify what is being toggled for queued cmd
        Name += " " + toggleName.Substring(2, toggleName.Length - 2); 
    }

    public void Execute()
    {
        // use reflection properties to flip toggle booleans by string
        System.Reflection.PropertyInfo toggleInfo = _characterInput.GetType().GetProperty(ToggleName);
        bool toggleValue = (bool)toggleInfo.GetValue(_characterInput, null);
        _characterInput.GetType().GetProperty(ToggleName).SetValue(_characterInput, !toggleValue);

        OnCompletion?.Invoke(this);
    }
}
