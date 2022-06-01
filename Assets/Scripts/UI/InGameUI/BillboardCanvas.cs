using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    private void LateUpdate()
    {
        if (Camera.main.transform == null)
            return;
            
        // ensure character stat bars always seem to be flat
        transform.LookAt(Camera.main.transform);
    }
}
