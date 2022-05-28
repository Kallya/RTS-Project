using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    private void LateUpdate()
    {
        // ensure character stat bars always seem to be flat
        transform.LookAt(POVManager.Instance.CurrVirtualCam.transform);
    }
}
