using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerControls : MonoBehaviour
{
    [SerializeField] private Image _autoAtkBtnImg;
    [SerializeField] private Image _queueBtnImg;
    private PlayerInfoUIManager _playerInfoUIManager;
    private static float s_bailRange = 50f;

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

    public void QueueBtnClick()
    {
        _playerInfoUIManager.CurrCmdProcessor.ExecuteCommand(new ChangeToggleCommand(
            _playerInfoUIManager.CurrCmdInput, 
            "IsQueueingCommands"
        ));
    }

    public void AutoAttackBtnClick()
    {
        _playerInfoUIManager.CurrCmdProcessor.ExecuteCommand(new ChangeToggleCommand(
            _playerInfoUIManager.CurrCmdInput, 
            "IsAutoAttacking"
        ));
    }

    public void UndoBtnClick()
    {
        _playerInfoUIManager.CurrCmdProcessor.Undo();
    }

    // character kills itself if not in certain range of enemies
    public void BailBtnClick()
    {
        GameObject currCharacter = _playerInfoUIManager.CurrCharacter;

        Collider[] collInRange = Physics.OverlapSphere(currCharacter.transform.position, s_bailRange);
        
        if (collInRange.Length == 0)
            return;

        foreach (Collider coll in collInRange)
        {
            if (coll.tag == "Enemy")
                return; // notify player can't bail
        }

        Destroy(currCharacter); // bailout - on death functionality in damageablecharacter
    }

    private void SetBtnColour(Image btnImg, bool toggleState)
    {
        if (toggleState == true)
            btnImg.color = Color.green;
        else
            btnImg.color = Color.white;
    }
}
