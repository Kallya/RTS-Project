using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerControls : MonoBehaviour
{
    private PlayerCommandInput playerInput;
    private CommandProcessor playerCmdProcessor;

    private void Start()
    {
        POVManager.Instance.OnPOVChanged += POVChanged;
    }

    public void OnQueueBtnClick()
    {
        playerInput.IsQueueingCommands = !playerInput.IsQueueingCommands;
    }

    public void OnAutoAttackBtnClick()
    {
        playerInput.IsAutoAttacking = !playerInput.IsAutoAttacking;
    }

    public void OnUndoBtnClick()
    {
        playerCmdProcessor.Undo();
    }

    public void OnBailBtnClick()
    {
        Debug.Log("Bail");
    }

    private void POVChanged(Transform currPlayer)
    {
        playerInput = currPlayer.GetComponent<PlayerCommandInput>();
        playerCmdProcessor = currPlayer.GetComponent<CommandProcessor>();
    }
}
