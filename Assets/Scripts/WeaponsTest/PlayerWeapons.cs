using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

enum Weapon : byte
{
    Gun,
    Shield,
    Bomb,
    Sword
}

public class PlayerWeapons : NetworkBehaviour
{
    public IEquipment ActiveEquipment { get; private set; }

    [SerializeField] private Weapons _weapons;
    private Dictionary<GameObject, IEquipment> _availableEquipmentInterfaces = new Dictionary<GameObject, IEquipment>();
    private List<GameObject> _availableWeapons;
    [SyncVar(hook=nameof(SetWeaponActives))] private int _activeWeaponSlot = 2;
    private GameObject[] _weaponsToAdd;

    private void Awake()
    {
        _weaponsToAdd = new GameObject[] {_weapons.Gun, _weapons.Shield, _weapons.Bomb, _weapons.Sword};
    }

    private void OnEnable()
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
    }

    public override void OnStartAuthority()
    {
        CmdAssignWeaponAuthority();
        CmdSwitchWeapon(1);
    }

    private void SetWeaponActives(int oldWeaponSlot, int newWeaponSlot)
    {
        // index is slot number minus 1
        GameObject newWeapon = _availableWeapons[newWeaponSlot-1];

        _availableWeapons[oldWeaponSlot-1]?.SetActive(false);

        if (newWeapon != null)
        {
            newWeapon.SetActive(true);
            ActiveEquipment = _availableEquipmentInterfaces[newWeapon];
        }
        else
            ActiveEquipment = null;
    }

    [Command]
    public void CmdSwitchWeapon(int weaponSlot)
    {
        // check if weapon slot exists
        if (weaponSlot < 1 || weaponSlot > _availableWeapons.Count)
            return;

        if (weaponSlot == _activeWeaponSlot)
            return;

        _activeWeaponSlot = weaponSlot;
    }

    [Command]
    private void CmdAssignWeaponAuthority()
    {
        foreach (GameObject weapon in _availableWeapons)
            weapon.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
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
        // Switch back automatically after losing a weapon?
        // SwitchWeapon(1);
    }
}
