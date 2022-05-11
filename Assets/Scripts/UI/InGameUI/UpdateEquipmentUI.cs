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
        CharacterEquipment currEquipment = PlayerInfoUIManager.Instance.CurrCharacterEquipment;
        
        SetEquipmentSprites(currEquipment);
        
        // reset colour on all slots
        foreach (Image slot in _equipmentSlots)
            slot.color = Color.white;

        SetSlotColour(currEquipment.ActiveEquipSlot, Color.yellow);
    }

    private void SetSlotColour(int slot, Color colour)
    {
        _equipmentSlots[slot-1].color = colour;
    }

    private void SetEquipmentSprites(CharacterEquipment currEquipment)
    {
        for (int i = 0; i < _equipmentSlots.Count; i++)
        {
            string weaponName = currEquipment.EquipmentToAdd[i];
            _equipmentSlots[i].sprite = Equipment.Instance.EquipmentReferences[weaponName]
                .GetComponent<IEquipment>()
                .EquipSprite;
        }
    }
}
