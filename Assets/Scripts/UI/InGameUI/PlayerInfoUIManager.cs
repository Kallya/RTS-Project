using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInfoUIManager : MonoBehaviour
{
    public static PlayerInfoUIManager Instance { get; private set; }
    public event System.Action<Stat> OnAnyStatChanged;
    public event System.Action<int, int> OnEquipSlotChanged;
    public CharacterStats CurrPlayerStats { get; private set; }
    public PlayerEquipment CurrPlayerWeapons { get; private set; }
    public GameObject CurrCharacter { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        POVManager.Instance.OnPOVChanged += POVChanged;
    }

    private void POVChanged(Transform currCharacter)
    {
        CurrCharacter = currCharacter.gameObject;

        // unsubscribe from previous character's stat/equipment changes
        if (CurrPlayerStats != null)
        {
            foreach (Stat stat in CurrPlayerStats.Stats)
                stat.OnStatChanged -= StatChanged;
        }
        
        if (CurrPlayerWeapons != null)
            CurrPlayerWeapons.OnEquipChanged -= WeaponChanged;

        // subscribe to current character's stat/equipment changes
        CurrPlayerStats = currCharacter.GetComponent<CharacterStats>();
        CurrPlayerWeapons = currCharacter.GetComponent<PlayerEquipment>();

        foreach (Stat stat in CurrPlayerStats.Stats)
            stat.OnStatChanged += StatChanged;

        CurrPlayerWeapons.OnEquipChanged += WeaponChanged;
        
        CurrPlayerStats.InitialiseStats();
    }

    private void WeaponChanged(int oldSlot, int newSlot)
    {
        OnEquipSlotChanged?.Invoke(oldSlot, newSlot);
    }

    private void StatChanged(Stat stat)
    {
        OnAnyStatChanged?.Invoke(stat);
    }
}
