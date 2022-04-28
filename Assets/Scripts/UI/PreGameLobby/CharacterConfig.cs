using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterConfig : MonoBehaviour
{
    public string[] GetWeaponConfig()
    {
        WeaponSelection[] weaponSelections = GetComponentsInChildren<WeaponSelection>();
        string[] weaponConfig = new string[weaponSelections.Length]; // array of names of character weapons
        
        for (int i = 0; i < weaponSelections.Length; i++)
        {
            TMP_Dropdown dropdown = weaponSelections[i].WeaponSelectionDropdown;
            weaponConfig[i] = dropdown.options[dropdown.value].text;

            dropdown.interactable = false; // disable dropdowns
        }

        return weaponConfig;
    }

    public string GetCharacterType()
    {
        CharacterTypeSelection characterTypeSelection = GetComponentInChildren<CharacterTypeSelection>();
        TMP_Dropdown dropdown = characterTypeSelection.CharacterTypeDropdown;
        string selection = dropdown.options[dropdown.value].text;

        dropdown.interactable = false;

        return selection;
    }
}
