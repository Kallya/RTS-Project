using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimationManager : MonoBehaviour
{
    private NavMeshAgent _characterNavMeshAgent;
    private CharacterEquipment _characterEquipment;
    private Animator _characterAnimator;

    // indexes for rifle weapons
    // to determine whether a rifle is being held
    // -1 if rifle is not equipped
    private int _assaultRifleIndex;
    private int _shotgunIndex;

    private void Awake()
    {
        _characterNavMeshAgent = GetComponent<NavMeshAgent>();
        _characterEquipment = GetComponent<CharacterEquipment>();
        _characterAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        _characterEquipment.OnEquipChanged += EquipChanged;

        _assaultRifleIndex = _characterEquipment.EquipmentToAdd.IndexOf("Assault Rifle");
        _shotgunIndex = _characterEquipment.EquipmentToAdd.IndexOf("Shotgun");
    }

    private void EquipChanged(int oldSlot, int newSlot)
    {
        // 1 if rifle held, 0 if anything else (inc. no equip)
        if (newSlot == _assaultRifleIndex || newSlot == _shotgunIndex)
            _characterAnimator.SetInteger("WeaponHeld", 1);
        else
            _characterAnimator.SetInteger("WeaponHeld", 0);
    }
}
