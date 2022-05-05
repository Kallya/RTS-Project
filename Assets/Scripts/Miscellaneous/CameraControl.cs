using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCam;
    private CinemachineFramingTransposer _virtualCamBody;
    // private static float s_rotSensitivity = 1500f;
    private static float s_zoomSensitivity = 500f;
    private static float s_moveSensitivity = 30f;
    private static float s_minZoom = 10f;
    private static float s_maxZoom = 50f;
    private static Vector3 s_screenCenter;

    private void Awake()
    {
        _virtualCam = GetComponent<CinemachineVirtualCamera>();
        _virtualCamBody = _virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();

        s_screenCenter = new Vector3(Screen.width/2, Screen.height/2, 0f);
    }
    
    private void Update()
    {
        // don't do anything if POV isn't set
        if (_virtualCam.Follow == null)
            return;

        if (Input.mouseScrollDelta.y != 0)
            AdjustCamZoom(s_zoomSensitivity, s_minZoom, s_maxZoom);

        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x <= 0 || mousePos.x >= Screen.width || mousePos.y <= 0 || mousePos.y >= Screen.height)
            MoveCamToMouse(s_moveSensitivity, s_screenCenter);

        if (Input.GetKeyDown(KeyCode.Space))
            CenterCamOnPlayer();
    }

    private void MoveCamToMouse(float moveSensitivity, Vector3 screenCenter)
    {
        Vector3 mouseDir = Vector3.Normalize(Input.mousePosition - screenCenter);
        float rotation = _virtualCam.Follow.eulerAngles.y - 45f;
        Vector3 camMoveVec = Quaternion.Euler(0f, 0f, rotation) * mouseDir * moveSensitivity * Time.deltaTime;

        _virtualCamBody.m_TrackedObjectOffset.x += camMoveVec.x;
        _virtualCamBody.m_TrackedObjectOffset.z += camMoveVec.y;
    }

    private void AdjustCamZoom(float zoomSensitivity, float minZoom, float maxZoom)
    {
        float camDist = _virtualCamBody.m_CameraDistance + -Input.mouseScrollDelta.y * zoomSensitivity * Time.deltaTime;
        _virtualCamBody.m_CameraDistance = Mathf.Clamp(camDist, minZoom, maxZoom);
    }

    private void CenterCamOnPlayer()
    {
        _virtualCamBody.m_TrackedObjectOffset.x = _virtualCamBody.m_TrackedObjectOffset.z = 0f;
    }
}
