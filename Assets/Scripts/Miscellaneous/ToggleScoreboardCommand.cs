using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScoreboardCommand : ICommand
{
    public event System.Action<ICommand> OnCompletion;

    private bool _activeState;

    public ToggleScoreboardCommand(bool activeState)
    {
        _activeState = activeState;
    }

    public void Execute()
    {
        ScoreManager.Instance.gameObject.SetActive(_activeState);
        OnCompletion.Invoke(this);
    }
}
