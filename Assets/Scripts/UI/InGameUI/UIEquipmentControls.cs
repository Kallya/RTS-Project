using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEquipmentControls : MonoBehaviour
{
    public void OnEquipBtnClick(int slotNumber)
    {
        GameObject currCharacter = PlayerInfoUIManager.Instance.CurrCharacter;

        CommandProcessor currCmdProcessor = currCharacter.GetComponent<CommandProcessor>();
        currCmdProcessor.ExecuteCommand(new SwitchWeaponCommand(currCharacter, slotNumber));
    }
}
