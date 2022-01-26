using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInfoUIManager : MonoBehaviour
{
    public static PlayerInfoUIManager Instance { get; private set; }
    public event System.Action<Stat> OnAnyStatChanged;
    public event System.Action<int> OnEquipSlotChanged;
    public CharacterStats CurrPlayerStats { get; private set; }
    public PlayerWeapons CurrPlayerWeapons { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        POVManager.Instance.OnPOVChanged += POVChanged;
    }

    private void POVChanged(Transform currPlayer)
    {
        // unsubscribe from previous character's stat/equipment changes
        if (CurrPlayerStats != null)
        {
            foreach (Stat stat in CurrPlayerStats.Stats)
                stat.OnStatChanged -= StatChanged;
        }
        
        if (CurrPlayerWeapons != null)
            CurrPlayerWeapons.OnWeaponChanged -= WeaponChanged;

        // subscribe to current character's stat/equipment changes
        CurrPlayerStats = currPlayer.GetComponent<CharacterStats>();
        CurrPlayerWeapons = currPlayer.GetComponent<PlayerWeapons>();

        foreach (Stat stat in CurrPlayerStats.Stats)
            stat.OnStatChanged += StatChanged;

        CurrPlayerWeapons.OnWeaponChanged += WeaponChanged;
        
        CurrPlayerStats.InitialiseStats();
    }

    private void WeaponChanged(int newSlot)
    {
        OnEquipSlotChanged?.Invoke(newSlot);
    }

    private void StatChanged(Stat stat)
    {
        OnAnyStatChanged?.Invoke(stat);
    }
}
