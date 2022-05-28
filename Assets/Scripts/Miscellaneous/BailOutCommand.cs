using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// character kills itself if not in range of enemies
public class BailOutCommand : IQueueableCommand
{
    public string Name { get; } = "Bail Out";
    public event System.Action<IQueueableCommand> OnCompletion;

    private GameObject _character;
    private float _bailRange = 50f;

    public BailOutCommand(GameObject character)
    {
        _character = character;
    }

    public void Execute()
    {
        // check for enemy players within bail out range
        Collider[] collInRange = Physics.OverlapSphere(_character.transform.position, _bailRange);

        foreach (Collider coll in collInRange)
        {
            if (coll.tag == "Enemy")
                return; // notify player can't bail
        }

        GameObject.Destroy(_character); // bailout - on death functionality in damageablecharacter
    }
}
