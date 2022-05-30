using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    private POVManager _povManager;
    private PlayerSettings _playerSettings;
    private float _zoomSensitivity;
    private float _panSensitivity;
    private float _rotSensitivity;
    private float _minZoom = 10f;
    private float _maxZoom = 50f;
    private Vector3 _screenCenter;
    private Quaternion _lastRot;
    private float _boundingDimension;

    private void Awake()
    {
        _screenCenter = new Vector3(Screen.width/2, Screen.height/2, 0f);
    }

    private void Start()
    {
        InitialiseSettings();

        _povManager = POVManager.Instance;
        PlayerSettings.OnSliderChanged += SliderChanged;
        
        _boundingDimension = _povManager.BoundingCollider.bounds.size.x / 2;
        // _lastRot = _povManager.CurrVirtualCam.Follow.rotation;
    }

    // initialise setting values based on player settings
    private void InitialiseSettings()
    {
        // if no. of settings increase
        // create a collection and loop through
        SliderChanged("Rotation Sensitivity");
        SliderChanged("Panning Sensitivity");
        SliderChanged("Zoom Sensitivity");
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
        Vector3 camMoveVec = Quaternion.Euler(0f, 0f, rotation) * mouseDir * _panSensitivity * Time.deltaTime;

        // only move offset on axix if the next cam position doesn't exceed the bounds
        // as if offset keeps moving in a direction while cam is confined (clamped to edges of map)
        // when moving back cam gets 'stuck' to edge (since it needs to reverse excess offset)
        Vector3 currCamPos = _povManager.CurrVirtualCam.transform.position;

        if (currCamPos.x + camMoveVec.x > -_boundingDimension && currCamPos.x + camMoveVec.x < _boundingDimension)
            _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.x += camMoveVec.x;
        if (currCamPos.z + camMoveVec.y > -_boundingDimension && currCamPos.z + camMoveVec.y < _boundingDimension)
            _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.z += camMoveVec.y;
    }

    // removes relative nature of tracked object offset
    // preventing camera from rotating when the player does (somewhat nauseating to watch)
    private void AdjustTrackedObjectOffset()
    { 
        float adjustmentRot = -_povManager.CurrVirtualCam.Follow.rotation.eulerAngles.y;
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

    private void SliderChanged(string settingName)
    {
        // each sensitivity scaled up based on testing
        // with menu allowing adjustment between 0 and 1
        switch (settingName)
        {
            case "Rotation Sensitivity":
                _rotSensitivity = PlayerSettings.s_SliderMappings[settingName] * 1000f;
                break;
            case "Panning Sensitivity":
                _panSensitivity = PlayerSettings.s_SliderMappings[settingName] * 100f;
                break;
            case "Zoom Sensitivity":
                _zoomSensitivity = PlayerSettings.s_SliderMappings[settingName] * 1000f;
                break;
        }
    }
}
