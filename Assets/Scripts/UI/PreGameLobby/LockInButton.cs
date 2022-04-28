using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

// all configs are separate fields because NetworkMessage does not support array of arrays
public struct CharacterConfigurationMessage : NetworkMessage
{
    public string[] CharacterTypes;
    public string[] Config1;
    public string[] Config2;
    public string[] Config3;
    public string[] Config4;
}

public class LockInButton : MonoBehaviour
{
    public void OnLockIn()
    {
        // disable all buttons
        Button[] buttons = transform.parent.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
            btn.interactable = false;

        CharacterConfigurationMessage msg = new CharacterConfigurationMessage();
        Tuple<string[], string[][]> characterConfigs = GetCharacterConfigs();
        string[][] weaponConfigs = characterConfigs.Item2;
        string[] characterTypes = characterConfigs.Item1;

        msg.CharacterTypes = characterTypes;
        // this seems really inefficient, is there a better way?
        // for some reason can't assign additively, other msg fields are nulled
        switch (weaponConfigs.Length)
        {
            case 1:
                msg.Config1 = weaponConfigs[0];
                break;
            case 2:
                msg.Config1 = weaponConfigs[0];
                msg.Config2 = weaponConfigs[1];
                break;
            case 3:
                msg.Config1 = weaponConfigs[0];
                msg.Config2 = weaponConfigs[1];
                msg.Config3 = weaponConfigs[2];
                break;
            case 4:
                msg.Config1 = weaponConfigs[0];
                msg.Config2 = weaponConfigs[1];
                msg.Config3 = weaponConfigs[2];
                msg.Config4 = weaponConfigs[3];
                break;
        }

        NetworkClient.Send(msg);
    }

    private Tuple<string[], string[][]> GetCharacterConfigs()
    {
        CharacterConfig[] characterConfigs = transform.parent.GetComponentsInChildren<CharacterConfig>();
        string[][] weaponConfigs = new string[characterConfigs.Length][]; // array of string arrays holding character weapon names
        string[] characterTypes = new string[characterConfigs.Length];

        for (int i = 0; i < characterConfigs.Length; i++)
        {
            weaponConfigs[i] = characterConfigs[i].GetWeaponConfig();
            characterTypes[i] = characterConfigs[i].GetCharacterType();
        }

        return new Tuple<string[], string[][]>(characterTypes, weaponConfigs);
    }
}
