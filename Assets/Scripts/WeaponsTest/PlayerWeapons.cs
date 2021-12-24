using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerWeapons : NetworkBehaviour
{
    public IEquipment ActiveEquipment { get; private set; }

    [SerializeField] private Weapons _weapons;
    private Dictionary<GameObject, IEquipment> _availableEquipmentInterfaces = new Dictionary<GameObject, IEquipment>();
    private List<GameObject> _availableWeapons;
    private GameObject _activeWeapon;
    private GameObject[] _weaponsToAdd;

    private void Awake()
    {
        _weaponsToAdd = new GameObject[] {_weapons.Gun, _weapons.Shield, _weapons.Bomb, _weapons.Sword};
    }

    [ServerCallback]
    public override void OnStartAuthority()
    {
        // need to automate Instantiation
        foreach (GameObject w in _weaponsToAdd)
        {
            GameObject weapon = Instantiate(w, transform);
            _availableEquipmentInterfaces.Add(weapon, weapon.GetComponent<IWeapon>());
        }

        foreach (KeyValuePair<GameObject, IEquipment> w in _availableEquipmentInterfaces)
        {
            w.Key.SetActive(false);

            // listen for when weapon breaks/can't be used anymore
            if (w.Value is ILimitedUseWeapon weapon)
                weapon.OnLimitReached += LimitReached;
        }

        _availableWeapons = new List<GameObject>(_availableEquipmentInterfaces.Keys);
        
        CmdSwitchWeapon(1);
    }

    [Command]
    public void CmdSwitchWeapon(int weaponSlot)
    {
        int slotIndex = weaponSlot - 1;

        // check if weapon slot exists
        if (slotIndex < 0 || slotIndex > _availableWeapons.Count - 1)
            return;

        // check if there is weapon in slot
        if (_availableWeapons[slotIndex] == null)
            return;
        
        if (_availableWeapons[slotIndex] != _activeWeapon)
        {
            _activeWeapon?.SetActive(false);
            _activeWeapon = _availableWeapons[slotIndex];
            _activeWeapon.SetActive(true);
            ActiveEquipment = _availableEquipmentInterfaces[_activeWeapon];
        }
    }

    public void LimitReached(GameObject weapon)
    {
        ILimitedUseWeapon w = (ILimitedUseWeapon)_availableEquipmentInterfaces[weapon];
        // unsubscribe from weapon as it is unattached
        w.OnLimitReached -= LimitReached;

        // null weapon slots so controls don't change
        _availableEquipmentInterfaces[weapon] = null;
        _availableWeapons[_availableWeapons.IndexOf(weapon)] = null;

        // null _activeWeapon to prevent deactivation
        _activeWeapon = null;
        CmdSwitchWeapon(1);
    }
}
