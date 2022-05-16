using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouseCommand : ICommand
{
    private Transform _characterTransform;

    public RotateToMouseCommand(Transform character)
    {
        _characterTransform = character;
    }

    // not accurate
    public void Execute()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
            _characterTransform.LookAt(new Vector3(hit.point.x, _characterTransform.position.y, hit.point.z));
    }
}
