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

    public void Execute()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
            _characterTransform.LookAt(new Vector3(hit.point.x, _characterTransform.position.y, hit.point.z));
    }
}
