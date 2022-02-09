using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCam;
    private CinemachineFramingTransposer _virtualCamBody;
    private static float s_rotSensitivity = 1000f;
    private static float s_zoomSensitivity = 500f;
    private static float s_moveSensitivity = 10f;
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
        if (Input.GetMouseButton(0))
            RotateCamHorizontal(s_rotSensitivity);

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
        float rotation = 360f - _virtualCam.Follow.rotation.eulerAngles.y;
        Vector3 camMoveVec = Quaternion.Euler(0f, 0f, rotation) * mouseDir * moveSensitivity * Time.deltaTime;

        _virtualCamBody.m_TrackedObjectOffset.x += camMoveVec.x;
        _virtualCamBody.m_TrackedObjectOffset.z += camMoveVec.y;
        
        /*
        Vector3 mouseDir = Vector3.Normalize(mousePos - screenCenter);
        Vector3 camMoveVec = mouseDir * moveSensitivity * Time.deltaTime;
        _virtualCamBody.m_ScreenX -= camMoveVec.x;
        _virtualCamBody.m_ScreenY += camMoveVec.y;
        */
    }

    private void RotateCamHorizontal(float rotSensitivity)
    {
        float horizontal = Input.GetAxis("Mouse X");
        _virtualCam.transform.Rotate(new Vector3(0f, horizontal * rotSensitivity * Time.deltaTime, 0f), Space.World);
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
