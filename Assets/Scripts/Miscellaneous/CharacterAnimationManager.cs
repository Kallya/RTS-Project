using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using Mirror;

public class CharacterAnimationManager : NetworkBehaviour
{
    private NavMeshAgent _characterNavMeshAgent;
    private CharacterEquipment _characterEquipment;
    private Animator _characterAnimator;
    private CommandProcessor _characterCmdProcessor;
    // cloak animation specific
    [SerializeField] private PlayableDirector _cloakPlayableDirector;

    // indexes for rifle weapons
    // to determine whether a rifle is being held
    // -1 if rifle is not equipped
    private Dictionary<string, List<int>> _weaponIndexes = new Dictionary<string, List<int>>() {
        {"Rifles", new List<int>()},
        {"Swords", new List<int>()}
    };

    [SerializeField] private List<string> _rifleNames; // assigned in inspector
    [SerializeField] private List<string> _swordNames; // different swords can be added here
    private Vector3 _lastPos;

    private void Awake()
    {
        _characterNavMeshAgent = GetComponent<NavMeshAgent>();
        _characterEquipment = GetComponent<CharacterEquipment>();
        _characterAnimator = GetComponent<Animator>();
        _characterCmdProcessor = GetComponent<CommandProcessor>();

        _lastPos = transform.position;
    }

    private void Start()
    {
        _characterEquipment.OnEquipChanged += EquipChanged;
        _characterCmdProcessor.OnQueueableCommandExecuted += QueueableCommandExecuted;

        int i = 0;
        foreach (string equip in _characterEquipment.EquipmentToAdd)
        {
            if (_rifleNames.Contains(equip))
                _weaponIndexes["Rifles"].Add(i);
            if (_swordNames.Contains(equip))
                _weaponIndexes["Swords"].Add(i);

            i++;
        }
    }

    private void Update()
    {
        // polling check for movement
        // (event based check (only setting when movecmd executed) had too many bugs)
        if (transform.position != _lastPos)
            _characterAnimator.SetBool("IsMoving", true);
        else
            _characterAnimator.SetBool("IsMoving", false);

        _lastPos = transform.position;

        // check for movement angle relative to angle character is facing
        // angle of movement direction relative to the character's front
        float movementAngleToFront = Vector3.SignedAngle(transform.forward, _characterNavMeshAgent.velocity, Vector3.up);

        if (movementAngleToFront < -45f && movementAngleToFront > -135f)
        {
            _characterAnimator.SetBool("MovingLeft", true);
            _characterAnimator.SetBool("MovingRight", false);
            _characterAnimator.SetBool("MovingBackward", false);
        }
        else if (movementAngleToFront > 45f && movementAngleToFront < 135f)
        {
            _characterAnimator.SetBool("MovingRight", true);
            _characterAnimator.SetBool("MovingLeft", false);
            _characterAnimator.SetBool("MovingBackward", false);
        }
        else if (movementAngleToFront < -135f && movementAngleToFront > 135f)
        {
            _characterAnimator.SetBool("MovingBackward", true);
            _characterAnimator.SetBool("MovingRight", false);
            _characterAnimator.SetBool("MovingLeft", false);
        }
        else
        {
            _characterAnimator.SetBool("MovingLeft", false);
            _characterAnimator.SetBool("MovingRight", false);
            _characterAnimator.SetBool("MovingBackward", false);
        }
    }

    private void QueueableCommandExecuted(IQueueableCommand command)
    {
        if (command is UtiliseCommand)
            _characterAnimator.SetTrigger("Utilised");
        else if (command is CloakCommand)
        {
            bool nextState = !_cloakPlayableDirector.gameObject.activeSelf;
            _cloakPlayableDirector.gameObject.SetActive(nextState);

            if (nextState == true)
                _cloakPlayableDirector.Play();
        }
            
    }

    private void Completion(IQueueableCommand command)
    {
        command.OnCompletion -= Completion;
    }

    private void EquipChanged(int oldSlot, int newSlot)
    {
        // 2 if sword held, 1 if rifle held, 0 if anything else (inc. no equip)
        if (_weaponIndexes["Swords"].Contains(newSlot-1))
            _characterAnimator.SetInteger("WeaponHeld", 2);
        else if (_weaponIndexes["Rifles"].Contains(newSlot-1))
            _characterAnimator.SetInteger("WeaponHeld", 1);
        else
            _characterAnimator.SetInteger("WeaponHeld", 0);
    }
}
