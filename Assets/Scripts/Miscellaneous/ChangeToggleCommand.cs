using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeToggleCommand : IQueueableCommand
{
    public string Name { get; } = "Toggle";
    public event System.Action<IQueueableCommand> OnCompletion;

    private CharacterCommandInput _characterInput;
    private string _toggleName;

    public ChangeToggleCommand(CharacterCommandInput characterInput, string toggleName)
    {
        _characterInput = characterInput;
        _toggleName = toggleName;
    }

    public void Execute()
    {
        // use reflection properties to flip toggle booleans by string
        System.Reflection.PropertyInfo toggleInfo = _characterInput.GetType().GetProperty(_toggleName);
        bool toggleValue = (bool)toggleInfo.GetValue(_characterInput, null);
        _characterInput.GetType().GetProperty(_toggleName).SetValue(_characterInput, !toggleValue);

        OnCompletion?.Invoke(this);
    }
}
