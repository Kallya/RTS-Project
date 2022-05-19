using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterEquipment : NetworkBehaviour
{
    public event System.Action<int, int> OnEquipChanged;
    public IEquipment ActiveEquipment { get; private set; }
    public readonly SyncList<string> EquipmentToAdd = new SyncList<string>();
    public int ActiveEquipSlot { get => _activeEquipSlot; }
    public GameObject RangeIndicatorSprite 
    {
        get => _rangeIndicatorSprite;
        set
        {
            _rangeIndicatorSprite = value;

            if (ActiveEquipment is IWeapon)
                SetRangeIndicator(((IWeapon)ActiveEquipment).Range);
        }
    }

    // all equipment parented to right hand
    [SerializeField] private Transform _rightHand;
    private Dictionary<GameObject, IEquipment> _availableEquipmentInterfaces = new Dictionary<GameObject, IEquipment>();
    private List<GameObject> _availableEquipment;    
    // initialise equipSlot to 2 to prevent indexing error for first change
    [SyncVar(hook=nameof(SetEquipmentActives))] private int _activeEquipSlot = 3;
    private GameObject _rangeIndicatorSprite;

    private void SetupEquipment()
    {
        foreach (string equipName in EquipmentToAdd)
        {
            GameObject equipPrefab = Equipment.Instance.EquipmentReferences[equipName];
            GameObject equippable = Instantiate(equipPrefab, _rightHand); // though if in hand, shooting weapons will lose accuracy when moving
            equippable.SetActive(false);
            _availableEquipmentInterfaces.Add(equippable, equippable.GetComponent<IEquipment>());
        }

        foreach (KeyValuePair<GameObject, IEquipment> w in _availableEquipmentInterfaces)
        {
            // listen for when equipment breaks/can't be used anymore
            if (w.Value is ILimitedUseEquippable equippable)
                equippable.OnLimitReached += LimitReached;
        }

        _availableEquipment = new List<GameObject>(_availableEquipmentInterfaces.Keys);
    }

    private void Start()
    {
        SetupEquipment();
        
        // ensure weapon is only switched once
        if (isServer)
            _activeEquipSlot = 1;
    }

    private void SetEquipmentActives(int oldSlot, int newSlot)
    {
        // check if equipment has been set (hook first called when field is declared)
        if (_availableEquipment == null)
            return;
        
        GameObject newEquip = _availableEquipment[newSlot-1];

        _availableEquipment[oldSlot-1]?.SetActive(false);

        if (newEquip != null)
        {
            newEquip.SetActive(true);
            ActiveEquipment = _availableEquipmentInterfaces[newEquip];

            if (ActiveEquipment is IWeapon)
            {
                IWeapon weapon = ActiveEquipment as IWeapon;
                SetRangeIndicator(weapon.Range);
            }
            else
                SetRangeIndicator(1f);
        }
        else
        {
            ActiveEquipment = null;
            SetRangeIndicator(1f);
        }

        OnEquipChanged?.Invoke(oldSlot, newSlot);
    }

    [Command]
    public void CmdSwitchEquipment(int equipSlot)
    {
        // check if equip slot exists
        if (equipSlot < 1 || equipSlot > _availableEquipment.Count)
            return;

        // prevent switching to non-existent equipment
        if (_availableEquipment[equipSlot-1] == null)
            return;

        if (equipSlot == _activeEquipSlot)
            return;

        _activeEquipSlot = equipSlot;
    }

    private void LimitReached(GameObject equippable)
    {
        if (!isServer)
            return;
            
        int equipIndex = _availableEquipment.IndexOf(equippable);

        CmdDisableEquippable(equipIndex);
    }

    [Command]
    private void CmdDisableEquippable(int equipIndex)
    {
        DisableEquippable(equipIndex);
        RpcDisableEquippable(equipIndex);
    }

    [ClientRpc]
    private void RpcDisableEquippable(int equipIndex)
    {
        if (isServer)
            return;
        
        DisableEquippable(equipIndex);
    }

    private void DisableEquippable(int equipIndex)
    {
        GameObject equippable = _availableEquipment[equipIndex];
        ILimitedUseEquippable equippedInterface = (ILimitedUseEquippable)_availableEquipmentInterfaces[equippable];
        // unsubscribe from weapon as it is unattached
        equippedInterface.OnLimitReached -= LimitReached;

        // null weapon slots so controls don't change
        _availableEquipmentInterfaces[equippable] = null;
        _availableEquipment[equipIndex] = null;
        ActiveEquipment = null;

        Destroy(equippable);
    }

    private void SetRangeIndicator(float range)
    {
        if (_rangeIndicatorSprite == null)
            return;

        _rangeIndicatorSprite.transform.localScale = new Vector3(range*0.22f, range*0.22f, 0.1f);
    }
}
