using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEquipmentControls : MonoBehaviour
{
    public void OnEquipBtnClick(int slotNumber)
    {
        CommandProcessor currCmdProcessor = PlayerInfoUIManager.Instance.CurrCmdProcessor;
        currCmdProcessor.ExecuteCommand(new SwitchWeaponCommand(PlayerInfoUIManager.Instance.CurrCharacterEquipment, slotNumber));
    }
}
