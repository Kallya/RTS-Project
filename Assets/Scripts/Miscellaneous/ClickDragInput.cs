using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ClickDragInput : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCam;
    private CinemachineFramingTransposer _virtualCamBody;

    [SerializeField]
    private static float s_rotateSensitivity = 360f;
    [SerializeField]
    private static float s_zoomSensitivity = 300f;

    private void Awake()
    {
        _virtualCam = GetComponent<CinemachineVirtualCamera>();
        _virtualCamBody = _virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float horizontal = Input.GetAxis("Mouse X");
            _virtualCam.transform.Rotate(new Vector3(0f, horizontal * s_rotateSensitivity * Time.deltaTime, 0f), Space.World);
        }

        if (Input.mouseScrollDelta.y != 0)
            _virtualCamBody.m_CameraDistance += -Input.mouseScrollDelta.y * s_zoomSensitivity * Time.deltaTime;
    }
}
