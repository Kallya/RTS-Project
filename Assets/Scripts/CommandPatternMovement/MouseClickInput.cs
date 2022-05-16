using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickInput : MonoBehaviour
{
    public static RaycastHit GetObjectHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);

        return hit;
    }

    public Vector3 GetMovementPosition()
    {
        RaycastHit target = GetObjectHit();

        if (target.transform.tag == "Ground")
            return target.point;
        else
            return transform.position;
    }
}
