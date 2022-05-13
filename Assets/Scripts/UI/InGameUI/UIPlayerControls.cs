using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerControls : MonoBehaviour
{
    [SerializeField] private Image _autoAtkBtnImg;
    [SerializeField] private Image _queueBtnImg;

    private void Start()
    {
        PlayerInfoUIManager.Instance.OnPOVChanged += POVChanged;
    }

    private void POVChanged()
    {
        SetBtnColour(_queueBtnImg, PlayerInfoUIManager.Instance.CurrCmdInput.IsQueueingCommands);
        SetBtnColour(_autoAtkBtnImg, PlayerInfoUIManager.Instance.CurrCmdInput.IsAutoAttacking);
    }

    public void OnQueueBtnClick()
    {
        PlayerInfoUIManager.Instance.CurrCmdProcessor.ExecuteCommand(new ChangeToggleCommand(
            PlayerInfoUIManager.Instance.CurrCmdInput, 
            "IsQueueingCommands"
        ));

        SetBtnColour(_queueBtnImg, PlayerInfoUIManager.Instance.CurrCmdInput.IsQueueingCommands);
    }

    public void OnAutoAttackBtnClick()
    {
        PlayerInfoUIManager.Instance.CurrCmdProcessor.ExecuteCommand(new ChangeToggleCommand(
            PlayerInfoUIManager.Instance.CurrCmdInput, 
            "IsAutoAttacking"
        ));

        SetBtnColour(_autoAtkBtnImg, PlayerInfoUIManager.Instance.CurrCmdInput.IsAutoAttacking);
    }

    public void OnUndoBtnClick()
    {
        PlayerInfoUIManager.Instance.CurrCmdProcessor.Undo();
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
