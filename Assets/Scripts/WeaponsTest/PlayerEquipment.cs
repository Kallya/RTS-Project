using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerEquipment : NetworkBehaviour
{
    public event System.Action<int, int> OnEquipChanged;
    public IEquipment ActiveEquipment { get; private set; }
    public readonly SyncList<string> EquipmentToAdd = new SyncList<string>();
    public int ActiveEquipSlot { get => _activeEquipSlot; }

    private Dictionary<GameObject, IEquipment> _availableEquipmentInterfaces = new Dictionary<GameObject, IEquipment>();
    private List<GameObject> _availableEquipment;    
    private GameObject _rangeIndicatorSprite;
    // initialise equipSlot to 2 to prevent indexing error for first change
    [SyncVar(hook=nameof(SetEquipmentActives))] private int _activeEquipSlot = 2;

    private void SetupEquipment()
    {
        foreach (string weaponName in EquipmentToAdd)
        {
            GameObject weaponPrefab = Weapons.Instance.WeaponReferences[weaponName];
            GameObject weapon = Instantiate(weaponPrefab, transform);
            _availableEquipmentInterfaces.Add(weapon, weapon.GetComponent<IWeapon>());
        }

        foreach (KeyValuePair<GameObject, IEquipment> w in _availableEquipmentInterfaces)
        {
            // disable all weapons initially
            w.Key.SetActive(false);

            // listen for when weapon breaks/can't be used anymore
            if (w.Value is ILimitedUseWeapon weapon)
                weapon.OnLimitReached += LimitReached;
        }

        _availableEquipment = new List<GameObject>(_availableEquipmentInterfaces.Keys);
    }

    private void Start()
    {
        SetupEquipment();

        _rangeIndicatorSprite = GetComponent<PlayerSpriteReferences>().RangeIndicatorSprite;
        
        if (netIdentity.hasAuthority == true)
            CmdSwitchEquipment(1);
    }

    private void SetEquipmentActives(int oldSlot, int newSlot)
    {
        // check if weapons have been set (hook first called when field is declared)
        if (_availableEquipment == null)
            return;
        
        // index is slot number minus 1
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
        }
        else
        {
            ActiveEquipment = null;
            SetRangeIndicator(1);
        }

        OnEquipChanged?.Invoke(oldSlot, newSlot);
    }

    [Command]
    public void CmdSwitchEquipment(int equipSlot)
    {
        // check if weapon slot exists
        if (equipSlot < 1 || equipSlot > _availableEquipment.Count)
            return;

        if (equipSlot == _activeEquipSlot)
            return;

        _activeEquipSlot = equipSlot;
    }

    private void LimitReached(GameObject weapon)
    {
        ILimitedUseWeapon w = (ILimitedUseWeapon)_availableEquipmentInterfaces[weapon];
        // unsubscribe from weapon as it is unattached
        w.OnLimitReached -= LimitReached;

        // null weapon slots so controls don't change
        _availableEquipmentInterfaces[weapon] = null;
        _availableEquipment[_availableEquipment.IndexOf(weapon)] = null;

        // Switch back automatically after losing a weapon?
        // SwitchWeapon(1);
    }

    private void SetRangeIndicator(float range)
    {
        _rangeIndicatorSprite.transform.localScale = new Vector3(range*2.5f, range*2.5f, 1f);
    }
}
