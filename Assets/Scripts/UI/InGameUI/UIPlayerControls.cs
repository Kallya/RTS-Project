using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerControls : MonoBehaviour
{
    [SerializeField] private Image _autoAtkBtnImg;
    [SerializeField] private Image _queueBtnImg;
    [SerializeField] private PlayerInfoUIManager _playerInfoUIManager;

    private void Start()
    {
        _playerInfoUIManager.OnPOVChanged += POVChanged;
        _playerInfoUIManager.OnToggleChanged += ToggleChanged;
    }

    private void ToggleChanged(string toggleName)
    {
        switch (toggleName)
        {
            case "IsQueueingCommands":
                SetBtnColour(_queueBtnImg, _playerInfoUIManager.CurrCmdInput.IsQueueingCommands);
                break;
            case "IsAutoAttacking":
                SetBtnColour(_autoAtkBtnImg, _playerInfoUIManager.CurrCmdInput.IsAutoAttacking);
                break;
        }
    }

    private void POVChanged()
    {
        SetBtnColour(_queueBtnImg, _playerInfoUIManager.CurrCmdInput.IsQueueingCommands);
        SetBtnColour(_autoAtkBtnImg, _playerInfoUIManager.CurrCmdInput.IsAutoAttacking);
    }

    public void QueueBtnClick()
    {
        _playerInfoUIManager.CurrCmdInput.ExecuteQueueableCmd(new ChangeToggleCommand(
            _playerInfoUIManager.CurrCmdInput, 
            "IsQueueingCommands"
        ));
    }

    public void AutoAttackBtnClick()
    {
        _playerInfoUIManager.CurrCmdInput.ExecuteQueueableCmd(new ChangeToggleCommand(
            _playerInfoUIManager.CurrCmdInput, 
            "IsAutoAttacking"
        ));
    }

    public void UndoBtnClick()
    {
        _playerInfoUIManager.CurrCmdProcessor.Undo();
    }

    public void BailBtnClick()
    {
        _playerInfoUIManager.CurrCmdInput.ExecuteQueueableCmd(new BailOutCommand(_playerInfoUIManager.CurrCharacter));
    }

    private void SetBtnColour(Image btnImg, bool toggleState)
    {
        if (toggleState == true)
            btnImg.color = Color.yellow;
        else
            btnImg.color = Color.white;
    }
}
