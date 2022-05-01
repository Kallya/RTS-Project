using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCommand : IQueueableCommand
{
    public string Name { get; } = "Activate Utility";
    public event System.Action<ICommand> OnCompletion;

    private IUtility _utility;

    public ActivateCommand(GameObject player)
    {
        _utility = (IUtility)player.GetComponent<PlayerEquipment>().ActiveEquipment;
    }

    public void Execute()
    {
        _utility.Activate();
        OnCompletion?.Invoke(this);
    }
}
