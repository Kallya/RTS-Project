using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapMovement : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int dimension; // length of one side of map (maps are square)
    private RectTransform _rectTransform;
    private static PointerEventData.InputButton _movementBtn = PointerEventData.InputButton.Right;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (CharacterCommandInput.InputsEnabled == false)
            return;

        if (pointerEventData.button != _movementBtn)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform, 
            pointerEventData.pressPosition, 
            pointerEventData.pressEventCamera,
            out Vector2 mapPos
        );

        Vector2 scaledMapPos = mapPos * (dimension/_rectTransform.rect.height); // ratio of map to minimap coordinate size
        
        CharacterCommandInput _currCmdInput = PlayerInfoUIManager.Instance.CurrCmdInput;
        MoveCommand moveCmd = new MoveCommand(
                    PlayerInfoUIManager.Instance.CurrCharacter,
                    new Vector3(scaledMapPos.x, 0f, scaledMapPos.y)
                );

        if (_currCmdInput.IsQueueingCommands)
            PlayerInfoUIManager.Instance.CurrCmdProcessor.QueueCommand(moveCmd);
        else
            PlayerInfoUIManager.Instance.CurrCmdProcessor.ExecuteCommand(moveCmd);
    }
}
