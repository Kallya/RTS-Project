using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    private POVManager _povManager;
    private static Quaternion s_initRot;
    private static float s_zoomSensitivity = 500f;
    private static float s_moveSensitivity = 30f;
    private static float s_minZoom = 10f;
    private static float s_maxZoom = 50f;
    private static Vector3 s_screenCenter;

    private void Awake()
    {
        s_screenCenter = new Vector3(Screen.width/2, Screen.height/2, 0f);
        _povManager = POVManager.Instance;
    }

    private void Update()
    {
        // don't do anything if POV isn't set
        if (_povManager.CurrVirtualCam == null || _povManager.CurrVirtualCam.Follow == null)
            return;

        if (Input.mouseScrollDelta.y != 0)
            AdjustCamZoom();

        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x <= 0 || mousePos.x >= Screen.width || mousePos.y <= 0 || mousePos.y >= Screen.height)
            MoveCamToMouse();

        if (Input.GetKeyDown(KeyCode.Space))
            CenterCamOnPlayer();
    }

    private void MoveCamToMouse()
    {
        Vector3 mouseDir = Vector3.Normalize(Input.mousePosition - s_screenCenter); // direction of mouse from center of screen
        float rotation = _povManager.CurrVirtualCam.Follow.eulerAngles.y - 45f; // adjustment for camera's viewing angle relative to current y rotation
        Vector3 camMoveVec = Quaternion.Euler(0f, 0f, rotation) * mouseDir * s_moveSensitivity * Time.deltaTime;

        _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.x += camMoveVec.x;
        _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.z += camMoveVec.y;
    }

    // removes relative nature of tracked object offset
    // preventing camera from rotating when the player does (somewhat nauseating to watch)
    private void AdjustTrackedObjectOffset()
    { 
        float adjustmentRot = -_povManager.CurrVirtualCam.Follow.rotation.eulerAngles.y;
        Vector3 adjustedOffset = Quaternion.Euler(0f, adjustmentRot, 0f) * _povManager.CurrVirtualCamBody.m_TrackedObjectOffset;

        _povManager.CurrVirtualCamBody.m_TrackedObjectOffset = adjustedOffset;
    }

    private void AdjustCamZoom()
    {
        float camDist = _povManager.CurrVirtualCamBody.m_CameraDistance + -Input.mouseScrollDelta.y * s_zoomSensitivity * Time.deltaTime;
        _povManager.CurrVirtualCamBody.m_CameraDistance = Mathf.Clamp(camDist, s_minZoom, s_maxZoom);
    }

    private void CenterCamOnPlayer()
    {
        _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.x = _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.z = 0f;
    }
}
