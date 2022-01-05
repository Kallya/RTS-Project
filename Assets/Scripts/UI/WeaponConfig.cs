using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponConfig : MonoBehaviour
{
    public string[] GetWeaponConfig()
    {
        WeaponSelection[] weaponSelections = GetComponentsInChildren<WeaponSelection>();
        string[] weaponConfig = new string[weaponSelections.Length]; // array of names of character weapons

        int i = weaponSelections.Length - 1;
        foreach (WeaponSelection selection in weaponSelections)
        {
            TMP_Dropdown dropdown = selection.WeaponSelectionDropdown;
            weaponConfig[i] = dropdown.options[dropdown.value].text;
            i -= 1;
        }

        return weaponConfig;
    }
}
