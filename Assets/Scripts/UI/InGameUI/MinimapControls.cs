using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapControls : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int dimension; // length of one side of map (maps are square)
    private RectTransform _rectTransform;
    private POVManager _povManager;
    private PlayerInfoUIManager _playerInfo;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _povManager = POVManager.Instance;
        _playerInfo = PlayerInfoUIManager.Instance;
    }

    private void Update()
    {
        if (Camera.main == null)
            return;

        transform.rotation = Quaternion.Euler(0f, 0f, Camera.main.transform.rotation.eulerAngles.y);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (CharacterCommandInput.InputsEnabled == false)
            return;

        // translate click position on screen to click position in minimap
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform, 
            pointerEventData.pressPosition, 
            pointerEventData.pressEventCamera,
            out Vector2 mapPos
        );

        // scale click pos on minimap in 3D space according to map dimesions
        // e.g. if minimap is 300x300 and actual map is 200x200
        // scale factor is 200/300
        Vector3 scaledClickPos = new Vector3(mapPos.x, 0f, mapPos.y) * (dimension / _rectTransform.rect.height);

        switch (pointerEventData.button)
        {
            case PointerEventData.InputButton.Right: // movement by minimap click
                MoveCommand moveCmd = new MoveCommand(
                        _playerInfo.CurrCharacter,
                        scaledClickPos
                    );

                if (_playerInfo.CurrCmdInput.IsQueueingCommands)
                    _playerInfo.CurrCmdProcessor.QueueCommand(moveCmd);
                else
                    _playerInfo.CurrCmdProcessor.ExecuteCommand(moveCmd);
                break;
            case PointerEventData.InputButton.Left:
                PanCamera(scaledClickPos);
                break;
        }
    }

    // pan camera in location of click on minimap
    private void PanCamera(Vector3 clickPos)
    {
        Vector3 panVec = clickPos - _playerInfo.CurrCharacter.transform.position;
        // adjustment for offset relative to character rotation
        Vector3 adjustedOffset = Quaternion.Euler(45f, 0f, -45f) * new Vector3(panVec.x, panVec.z, 0f);

        _povManager.CurrCamOffset.m_Offset = adjustedOffset;
    }
}
