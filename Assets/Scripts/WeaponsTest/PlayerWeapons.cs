using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    public IEquipment ActiveEquipment { get; private set; }

    [SerializeField]
    private Weapons _weapons;
    // change the two lists to a dictionary where _availableWeapons references interface
    [SerializeField]
    private Dictionary<GameObject, IEquipment> _availableEquipmentInterfaces = new Dictionary<GameObject, IEquipment>();
    private List<GameObject> _availableWeapons;
    private GameObject _activeWeapon;
    private GameObject[] _weaponsToAdd;

    private void Awake()
    {
        _weaponsToAdd = new GameObject[] {_weapons.Gun, _weapons.Shield, _weapons.Bomb, _weapons.Sword};

        // need to automate Instantiation
        foreach (GameObject w in _weaponsToAdd)
        {
            GameObject g = Instantiate(w, transform);
            _availableEquipmentInterfaces.Add(g, g.GetComponent<IWeapon>());
        }

        foreach (KeyValuePair<GameObject, IEquipment> w in _availableEquipmentInterfaces)
        {
            w.Key.SetActive(false);

            if (w.Value is ILimitedUseWeapon weapon)
                weapon.OnLimitReached += LimitReached;
        }

        _availableWeapons = new List<GameObject>(_availableEquipmentInterfaces.Keys);
    }

    private void Start()
    {
        SwitchWeapon(1);
    }

    public void SwitchWeapon(int weaponSlot)
    {
        int slotIndex = weaponSlot - 1;

        // check if weapon in slot
        if (slotIndex < 0 || slotIndex > _availableWeapons.Count - 1)
            return;
        
        if (_availableWeapons[slotIndex] != _activeWeapon)
        {
            _activeWeapon?.SetActive(false);
            _activeWeapon = _availableWeapons[slotIndex];
            _activeWeapon?.SetActive(true);
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
        SwitchWeapon(1);
    }
}
