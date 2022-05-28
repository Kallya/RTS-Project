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
        float camDist = Mathf.Sqrt(2*Mathf.Pow(Camera.main.transform.position.y-0.5f, 2)); // distance of camera from map plane
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDist)
        );

        _characterTransform.LookAt(new Vector3(mousePos.x, _characterTransform.position.y, mousePos.z));
    }
}
