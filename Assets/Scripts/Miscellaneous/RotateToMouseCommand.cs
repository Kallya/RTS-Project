using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouseCommand : ICommand
{
    public event System.Action<ICommand> OnCompletion;

    private Transform _playerTransform;

    public RotateToMouseCommand (GameObject player)
    {
        _playerTransform = player.transform;
    }

    public void Execute()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
            _playerTransform.LookAt(new Vector3(hit.point.x, _playerTransform.position.y, hit.point.z));

        OnCompletion?.Invoke(this);
    }
}
