using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerControls : MonoBehaviour
{
    private CharacterCommandInput _playerInput;
    private CommandProcessor _playerCmdProcessor;

    private void Start()
    {
        POVManager.Instance.OnPOVChanged += POVChanged;
    }

    public void OnQueueBtnClick(Image queueBtnImg)
    {
        _playerCmdProcessor.ExecuteCommand(new ChangeToggleCommand(_playerInput, "IsQueueingCommands"));
        SetBtnColour(queueBtnImg, _playerInput.IsQueueingCommands);
    }

    public void OnAutoAttackBtnClick(Image autoAtkBtnImg)
    {
        _playerCmdProcessor.ExecuteCommand(new ChangeToggleCommand(_playerInput, "IsAutoAttacking"));
        SetBtnColour(autoAtkBtnImg, _playerInput.IsAutoAttacking);
    }

    public void OnUndoBtnClick()
    {
        _playerCmdProcessor.Undo();
    }

    public void OnBailBtnClick()
    {
        Debug.Log("Bail");
    }

    private void POVChanged(Transform currPlayer)
    {
        _playerInput = currPlayer.GetComponent<CharacterCommandInput>();
        _playerCmdProcessor = currPlayer.GetComponent<CommandProcessor>();
    }

    private void SetBtnColour(Image btnImg, bool toggle)
    {
        if (toggle == true)
            btnImg.color = Color.green;
        else
            btnImg.color = Color.white;
    }
}
