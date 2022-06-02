using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateEquipmentUI : MonoBehaviour
{
    // both lists must be in order of slots from 1 to 4
    [SerializeField] private List<Image> _equipmentSlotImgs; // imgs to set weapon sprites
    [SerializeField] private List<Image> _equipmentSlotBtns; // buttons to change colour indicating active weapon
    [SerializeField] private PlayerInfoUIManager _playerInfoUIManager;

    private void Start()
    {
        _playerInfoUIManager.OnEquipSlotChanged += EquipSlotChanged;
        _playerInfoUIManager.OnPOVChanged += POVChanged;
    }

    private void EquipSlotChanged(int oldSlot, int newSlot)
    {
        SetSlotColour(oldSlot, Color.white);
        SetSlotColour(newSlot, Color.green);
    }

    private void POVChanged()
    {
        CharacterEquipment currEquipment = _playerInfoUIManager.CurrCharacterEquipment;
        
        SetEquipmentSprites(currEquipment);
        
        // reset colour on all slots
        foreach (Image slot in _equipmentSlotBtns)
            slot.color = Color.white;

        SetSlotColour(currEquipment.ActiveEquipSlot, Color.green);
    }

    private void SetSlotColour(int slot, Color colour)
    {
        _equipmentSlotBtns[slot-1].color = colour;
    }

    private void SetEquipmentSprites(CharacterEquipment currEquipment)
    {
        for (int i = 0; i < _equipmentSlotImgs.Count; i++)
        {
            string weaponName = currEquipment.EquipmentToAdd[i];
            _equipmentSlotImgs[i].sprite = Equipment.Instance.EquipmentReferences[weaponName]
                .GetComponent<IEquipment>()
                .EquipSprite;
        }
    }
}
