using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickInput : MonoBehaviour
{
    public static RaycastHit GetObjectHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        Physics.Raycast(ray, out hit);

        return hit;
    }

    public Vector3 GetMovementPosition()
    {
        RaycastHit target = GetObjectHit();
        Vector3 dest;
        
        switch (target.transform.tag)
        {
            case "Ground":
                dest = target.point;
                break;
            case "Enemy":
                dest = target.transform.position;
                break;
            default:
                dest = transform.position;
                break;
        }

        return dest;
    }
}
