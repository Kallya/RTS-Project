using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{
    private int _camCollLayer = 9;
    private int _mapLayer = 8;

    private void OnTriggerEnter(Collider coll)
    {
        coll.gameObject.layer = _camCollLayer;
    }

    private void OnTriggerExit(Collider coll)
    {
        coll.gameObject.layer = _mapLayer;
    }
}

