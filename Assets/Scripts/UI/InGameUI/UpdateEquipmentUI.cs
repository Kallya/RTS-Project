using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateEquipmentUI : MonoBehaviour
{
    [SerializeField] private List<Image> _equipmentSlots;

    private void Start()
    {
        PlayerInfoUIManager.Instance.OnEquipSlotChanged += EquipSlotChanged;
        POVManager.Instance.OnPOVChanged += POVChanged;
    }

    private void EquipSlotChanged(int oldSlot, int newSlot)
    {
        SetSlotColour(oldSlot, Color.white);
        SetSlotColour(newSlot, Color.yellow);
    }

    private void POVChanged(Transform currCharacter)
    {
        PlayerWeapons currWeapons = PlayerInfoUIManager.Instance.CurrPlayerWeapons;
        
        SetEquipmentSprites(currWeapons);
        
        // reset colour on all slots
        foreach (Image slot in _equipmentSlots)
            slot.color = Color.white;

        SetSlotColour(currWeapons.ActiveWeaponSlot, Color.yellow);
    }

    private void SetSlotColour(int slot, Color colour)
    {
        _equipmentSlots[slot-1].color = colour;
    }

    private void SetEquipmentSprites(PlayerWeapons currWeapons)
    {
        for (int i = 0; i < _equipmentSlots.Count; i++)
        {
            string weaponName = currWeapons.WeaponsToAdd[i];
            _equipmentSlots[i].sprite = Weapons.Instance.WeaponReferences[weaponName]
                .GetComponent<IEquipment>()
                .EquipSprite;
        }
    }
}
