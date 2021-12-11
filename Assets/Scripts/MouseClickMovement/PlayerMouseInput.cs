using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouseInput : MonoBehaviour
{
    public Vector3 NextPosition { get; private set; }

    private void Awake()
    {
        NextPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
            NextPosition = GetMovementPosition();
    }

    private Vector3 GetMovementPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Ground")
            return hit.point;
        else
            // better solution available?
            return transform.position;
    }
}
