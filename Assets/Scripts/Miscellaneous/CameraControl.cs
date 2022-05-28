using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    private POVManager _povManager;
    private PlayerSettings _playerSettings;
    private float _zoomSensitivity = 500f;
    private float _moveSensitivity = 30f;
    private float _minZoom = 10f;
    private float _maxZoom = 50f;
    private float _rotSensitivity = 200f;
    private Vector3 _screenCenter;
    private Quaternion _lastRot;
    private bool _startedDebugging;

    private void Awake()
    {
        _screenCenter = new Vector3(Screen.width/2, Screen.height/2, 0f);
    }

    private void Start()
    {
        _povManager = POVManager.Instance;
        
        _lastRot = _povManager.CurrVirtualCam.Follow.rotation;
    }

    private void Update()
    {
        if (CharacterCommandInput.InputsEnabled == false)
            return;

        // don't do anything if POV isn't set
        if (_povManager.CurrVirtualCam == null || _povManager.CurrVirtualCam.Follow == null)
            return;

        if (Input.mouseScrollDelta.y != 0)
            AdjustCamZoom();

        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x <= 0 || mousePos.x >= Screen.width || mousePos.y <= 0 || mousePos.y >= Screen.height)
            MoveCamToMouse();

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Center Camera"]))
            CenterCamOnPlayer();
        
        if (Input.GetKey(PlayerSettings.s_HotkeyMappings["Rotate Camera"]))
        {
            float horizontal = Input.GetAxis("Mouse X");
            RotateCamHorizontal(horizontal);
        }
    }
/*
    private void LateUpdate()
    {
        if (_lastRot != _povManager.CurrVirtualCam.Follow.rotation && _startedDebugging == false)
        {
            StartCoroutine(RotationDebug());
            _startedDebugging = true;
        }

        _lastRot = _povManager.CurrVirtualCam.Follow.rotation;
    }
    */

    private void MoveCamToMouse()
    {
        Vector3 mouseDir = Vector3.Normalize(Input.mousePosition - _screenCenter); // direction of mouse from center of screen
        // adjustment for camera's viewing angle relative to current character's y rotation
        float rotation = _povManager.CurrVirtualCam.Follow.eulerAngles.y - _povManager.CurrVirtualCam.transform.rotation.eulerAngles.y;
        Vector3 camMoveVec = Quaternion.Euler(0f, 0f, rotation) * mouseDir * _moveSensitivity * Time.deltaTime;

        _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.x += camMoveVec.x;
        _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.z += camMoveVec.y;
    }

    // removes relative nature of tracked object offset
    // preventing camera from rotating when the player does (somewhat nauseating to watch)
    private void AdjustTrackedObjectOffset()
    { 
        float adjustmentRot = -_povManager.CurrVirtualCam.transform.rotation.eulerAngles.y;
        Vector3 adjustedOffset = Quaternion.Euler(0f, adjustmentRot, 0f) * _povManager.CurrVirtualCamBody.m_TrackedObjectOffset;
        _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.x = adjustedOffset.x;
        _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.z = adjustedOffset.z;
    }

    private void AdjustCamZoom()
    {
        float camDist = _povManager.CurrVirtualCamBody.m_CameraDistance + -Input.mouseScrollDelta.y * _zoomSensitivity * Time.deltaTime;
        _povManager.CurrVirtualCamBody.m_CameraDistance = Mathf.Clamp(camDist, _minZoom, _maxZoom);
    }

    private void CenterCamOnPlayer()
    {
        _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.x = _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.z = 0f;
    }

    private void RotateCamHorizontal(float mouseX)
    {
        _povManager.CurrVirtualCam.transform.Rotate(new Vector3(0f, mouseX * _rotSensitivity * Time.deltaTime, 0f), Space.World);
    }

    private IEnumerator RotationDebug()
    {
        Debug.Log($"character rotation: {_povManager.CurrVirtualCam.Follow.rotation.eulerAngles}");
        Debug.Log($"current tracked object offset: {_povManager.CurrVirtualCamBody.m_TrackedObjectOffset}");

        yield return new WaitUntil(() => _lastRot == _povManager.CurrVirtualCam.Follow.rotation);

        float adjustmentRot = -_povManager.CurrVirtualCam.transform.rotation.eulerAngles.y;
        Vector3 adjustedOffset = Quaternion.Euler(0f, adjustmentRot, 0f) * _povManager.CurrVirtualCamBody.m_TrackedObjectOffset;

        _povManager.CurrVirtualCamBody.m_TrackedObjectOffset = adjustedOffset;
        _startedDebugging = false;
        Debug.Log($"character rotation: {_povManager.CurrVirtualCam.Follow.rotation.eulerAngles}");
        Debug.Log($"actual tracked object offset: {_povManager.CurrVirtualCamBody.m_TrackedObjectOffset}");
        Debug.Log($"adjusted tracked object offset: {adjustedOffset}");
    }
}
