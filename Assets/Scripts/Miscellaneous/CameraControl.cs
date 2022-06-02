using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private POVManager _povManager;
    private PlayerSettings _playerSettings;
    private float _zoomSensitivity;
    private float _panSensitivity;
    private float _rotSensitivity;
    private float _minZoom = 10f;
    private float _maxZoom = 100f;
    private float _panEdgeOffset = 10f;
    private Vector3 _screenCenter;

    private void Awake()
    {
        _screenCenter = new Vector3(Screen.width/2, Screen.height/2, 0f);
    }

    private void Start()
    {
        InitialiseSettings();
        
        PlayerSettings.OnSliderChanged += SliderChanged;
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
        if (mousePos.x <= _panEdgeOffset || mousePos.x >= Screen.width - _panEdgeOffset || mousePos.y <= _panEdgeOffset || mousePos.y >= Screen.height - _panEdgeOffset)
            MoveCamToMouse();
        

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Center Camera"]))
            CenterCamOnPlayer();
        
        if (Input.GetKey(PlayerSettings.s_HotkeyMappings["Rotate Camera"]))
        {
            float horizontal = Input.GetAxis("Mouse X");
            RotateCamHorizontal(horizontal);
        }
    }

    private void MoveCamToMouse()
    {
        Vector3 mouseDir = Vector3.Normalize(Input.mousePosition - _screenCenter); // direction of mouse from center of screen
        Vector3 camMoveVec = Quaternion.Euler(45f, 0f, 0f) * mouseDir * _panSensitivity * Time.deltaTime; // rotated to account for camera rotation

        _povManager.CurrCamOffset.m_Offset.x += camMoveVec.x;
        _povManager.CurrCamOffset.m_Offset.y += camMoveVec.y;
        _povManager.CurrCamOffset.m_Offset.z += camMoveVec.z;
    }

    private void AdjustCamZoom()
    {
        float camDist = _povManager.CurrVirtualCamBody.m_CameraDistance + -Input.mouseScrollDelta.y * _zoomSensitivity * Time.deltaTime;
        _povManager.CurrVirtualCamBody.m_CameraDistance = Mathf.Clamp(camDist, _minZoom, _maxZoom);
    }

    private void CenterCamOnPlayer()
    {
        // _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.x = _povManager.CurrVirtualCamBody.m_TrackedObjectOffset.z = 0f;
        _povManager.CurrCamOffset.m_Offset = Vector3.zero;
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
