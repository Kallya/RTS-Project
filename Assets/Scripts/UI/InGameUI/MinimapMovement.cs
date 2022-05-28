using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapMovement : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int dimension; // length of one side of map (maps are square)
    private RectTransform _rectTransform;
    private PointerEventData.InputButton _movementBtn = PointerEventData.InputButton.Right;
    private PointerEventData.InputButton _camPanBtn = PointerEventData.InputButton.Left;
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

    public void OnPointerClick(PointerEventData pointerEventData)
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
        Vector3 scaledMapPos = new Vector3(mapPos.x, 0f, mapPos.y) * (dimension / _rectTransform.rect.height);

        if (pointerEventData.button == _movementBtn)
        {
            MoveCommand moveCmd = new MoveCommand(
                        _playerInfo.CurrCharacter,
                        scaledMapPos
                    );

            if (_playerInfo.CurrCmdInput.IsQueueingCommands)
                _playerInfo.CurrCmdProcessor.QueueCommand(moveCmd);
            else
                _playerInfo.CurrCmdProcessor.ExecuteCommand(moveCmd);
        }
        else if (pointerEventData.button == _camPanBtn)
        {
            Vector3 panVec = scaledMapPos - _playerInfo.CurrCharacter.transform.position;
            // adjustment for offset relative to character rotation
            float rotation = _povManager.CurrVirtualCam.transform.rotation.eulerAngles.y - _playerInfo.CurrCharacter.transform.eulerAngles.y;
            Vector3 adjustedOffset = Quaternion.Euler(0f, rotation, 0f) * panVec;

            _povManager.CurrVirtualCamBody.m_TrackedObjectOffset = adjustedOffset;
        }
        
    }
}
