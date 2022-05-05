using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    private Quaternion _initRot;

    private void Awake()
    {
        _initRot = transform.rotation;
    }
    private void Update()
    {
        transform.rotation = _initRot;
    }
}
