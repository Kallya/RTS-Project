using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeToggleCommand : IQueueableCommand
{
    public string Name { get; } = "Toggle";
    public event System.Action<ICommand> OnCompletion;

    private PlayerCommandInput _playerInput;
    private string _toggleName;

    public ChangeToggleCommand(PlayerCommandInput playerInput, string toggleName)
    {
        _playerInput = playerInput;
        _toggleName = toggleName;
    }

    public void Execute()
    {
        System.Reflection.PropertyInfo toggleInfo = _playerInput.GetType().GetProperty(_toggleName);
        bool toggleValue = (bool)toggleInfo.GetValue(_playerInput, null);
        _playerInput.GetType().GetProperty(_toggleName).SetValue(_playerInput, !toggleValue);

        OnCompletion?.Invoke(this);
    }
}
