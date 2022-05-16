using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerControls : MonoBehaviour
{
    [SerializeField] private Image _autoAtkBtnImg;
    [SerializeField] private Image _queueBtnImg;
    private PlayerInfoUIManager _playerInfoUIManager;

    private void Start()
    {
        _playerInfoUIManager = PlayerInfoUIManager.Instance;

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

    public void OnQueueBtnClick()
    {
        _playerInfoUIManager.CurrCmdProcessor.ExecuteCommand(new ChangeToggleCommand(
            _playerInfoUIManager.CurrCmdInput, 
            "IsQueueingCommands"
        ));
    }

    public void OnAutoAttackBtnClick()
    {
        _playerInfoUIManager.CurrCmdProcessor.ExecuteCommand(new ChangeToggleCommand(
            _playerInfoUIManager.CurrCmdInput, 
            "IsAutoAttacking"
        ));
    }

    public void OnUndoBtnClick()
    {
        _playerInfoUIManager.CurrCmdProcessor.Undo();
    }

    public void OnBailBtnClick()
    {
        Debug.Log("Bail");
    }

    private void SetBtnColour(Image btnImg, bool toggleState)
    {
        if (toggleState == true)
            btnImg.color = Color.green;
        else
            btnImg.color = Color.white;
    }
}
