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
    private List<int> _rifleIndexes = new List<int>();
    [SerializeField] private List<string> _rifleNames; // assigned in inspector

    private void Awake()
    {
        _characterNavMeshAgent = GetComponent<NavMeshAgent>();
        _characterEquipment = GetComponent<CharacterEquipment>();
        _characterAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        _characterEquipment.OnEquipChanged += EquipChanged;

        int i = 0;
        foreach (string equip in _characterEquipment.EquipmentToAdd)
        {
            if (_rifleNames.Contains(equip))
                _rifleIndexes.Add(i);
            
            i++;
        }
    }

    private void Update()
    {
        // is there a more efficient way (where i'm not uselessly setting parameters)
        if (DestinationReached())
            _characterAnimator.SetBool("IsMoving", false);
        else
            _characterAnimator.SetBool("IsMoving", true);
    }

    private void EquipChanged(int oldSlot, int newSlot)
    {
        Debug.Log(newSlot);
        // 1 if rifle held, 0 if anything else (inc. no equip)
        if (_rifleIndexes.Contains(newSlot-1))
            _characterAnimator.SetInteger("WeaponHeld", 1);
        else
            _characterAnimator.SetInteger("WeaponHeld", 0);
    }

    private bool DestinationReached()
    {
        // reference https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
        if (!_characterNavMeshAgent.pathPending)
        {
            if (_characterNavMeshAgent.remainingDistance <= _characterNavMeshAgent.stoppingDistance)
            {
                if (!_characterNavMeshAgent.hasPath || _characterNavMeshAgent.velocity.sqrMagnitude == 0f)
                    return true;
            }
        }

        return false;
    }
}
