using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickInput : MonoBehaviour
{
    public Vector3 GetMovementPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Ground")
            return hit.point;
        else
            return transform.position;
    }
}
