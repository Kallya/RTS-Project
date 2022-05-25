using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickInput
{
    public static RaycastHit GetObjectHit(Camera cam)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);

        return hit;
    }

    public static Vector3 GetMovementPosition(Transform currCharacter, Camera cam)
    {
        RaycastHit target = GetObjectHit(cam);

        if (target.transform.tag == "Ground")
            return target.point;
        else
            return currCharacter.position;
    }
}
