using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct WeaponSelectionMessage : NetworkMessage
{
    public string[][] weaponConfigs;
}

public class LockInButton : MonoBehaviour
{
    public void OnLockIn()
    {
        WeaponSelectionMessage msg = new WeaponSelectionMessage { weaponConfigs = GetWeaponConfigs() };
        
        NetworkClient.Send(msg);
    }

    private string[][] GetWeaponConfigs()
    {
        WeaponConfig[] weaponConfigs = transform.parent.GetComponentsInChildren<WeaponConfig>();
        string[][] configs = new string[weaponConfigs.Length][]; // array of string arrays holding character weapon names

        int i = weaponConfigs.Length - 1;
        foreach (WeaponConfig config in weaponConfigs)
        {
            configs[i] = config.GetWeaponConfig();
            i -= 1;
        }

        return configs;
    }
}
