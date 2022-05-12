using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

// manager holds references for other ui elements to use too
public class PlayerInfoUIManager : MonoBehaviour
{
    public static PlayerInfoUIManager Instance { get; private set; }
    public event System.Action<Stat> OnAnyStatChanged;
    public event System.Action<int, int> OnEquipSlotChanged;
    public CharacterStats CurrCharacterStats { get; private set; }
    public CharacterEquipment CurrCharacterEquipment { get; private set; }
    public GameObject CurrCharacter { get; private set; }
    public CommandProcessor CurrCmdProcessor { get; private set; }
    public CharacterCommandInput CurrCmdInput { get; private set; }
    
    [SerializeField] private Image _characterPortrait;

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
        if (CurrCharacterStats != null)
        {
            foreach (Stat stat in CurrCharacterStats.Stats)
                stat.OnStatChanged -= StatChanged;
        }
        
        if (CurrCharacterEquipment!= null)
            CurrCharacterEquipment.OnEquipChanged -= WeaponChanged;

        CurrCharacterStats = currCharacter.GetComponent<CharacterStats>();
        CurrCharacterEquipment = currCharacter.GetComponent<CharacterEquipment>();
        CurrCmdProcessor = currCharacter.GetComponent<CommandProcessor>();
        CurrCmdInput = currCharacter.GetComponent<CharacterCommandInput>();

        // subscribe to current character's stat/equipment changes
        foreach (Stat stat in CurrCharacterStats.Stats)
            stat.OnStatChanged += StatChanged;

        CurrCharacterEquipment.OnEquipChanged += WeaponChanged;
        CurrCharacterStats.InitialiseStats();

        _characterPortrait.sprite = CurrCharacterStats.CharacterSprite;
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
