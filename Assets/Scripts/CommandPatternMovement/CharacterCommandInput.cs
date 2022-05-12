using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MouseClickInput))]
public class CharacterCommandInput : NetworkBehaviour
{
    public bool IsQueueingCommands { get; set; } = false;
    public bool IsAutoAttacking { get; set; } = false;
    public bool IsCloaked { get; set; } = false;
    public bool IsTargeting { get; set; } = false;

    private CommandProcessor _commandProcessor;
    private MouseClickInput _mouseInput;
    private CharacterEquipment _characterEquipment;
    private RaycastHit _objectHit;
    private static int _cloakInterval = 3; // interval between cloak cost reduction
    private double _lastCloakCostTime;

    // consider depending on how it feels to play
/*
    private void OnEnable()
    {
        IsQueueingCommands = false;
    }
*/
    private void Awake()
    {
        _commandProcessor = GetComponent<CommandProcessor>();
        _mouseInput = GetComponent<MouseClickInput>();
        _characterEquipment = GetComponent<CharacterEquipment>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Don't allow the server to interact with game through inputs
        if (isServerOnly)
            return;
        // need to rethink this
        if (IsQueueingCommands)
        {
            if (Input.GetMouseButtonDown(1))
            {
                _objectHit = MouseClickInput.GetObjectHit();

                if (_objectHit.transform.tag == "Ground")
                    _commandProcessor.QueueCommand(new MoveCommand(gameObject, _mouseInput.GetMovementPosition()));
                else if (_objectHit.transform.tag == "Enemy")
                    _commandProcessor.QueueCommand(new ChangeToggleCommand(this, "IsTargeting"));
            }
                
            if (Input.GetKeyDown(KeyCode.T)) 
                _commandProcessor.Undo();

            if (Input.GetKeyDown(KeyCode.D))
                _commandProcessor.QueueCommand(new ChangeToggleCommand(this, "IsAutoAttacking"));

            if (Input.GetKeyDown(KeyCode.C))
                ChangeCloak();

            if (Input.GetKey(KeyCode.F))
            {
                if (_characterEquipment.ActiveEquipment != null)
                    _commandProcessor.QueueCommand(new UtiliseCommand(_characterEquipment, netId));
                else
                    Debug.Log("You are not holding anything that is useable!");
            }

            if (Input.GetKeyDown(KeyCode.Q))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(_characterEquipment, 1));

            if (Input.GetKeyDown(KeyCode.W))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(_characterEquipment, 2));

            if (Input.GetKeyDown(KeyCode.E))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(_characterEquipment, 3));

            if (Input.GetKeyDown(KeyCode.R))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(_characterEquipment, 4));
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                _objectHit = MouseClickInput.GetObjectHit();
                if (_objectHit.transform.tag == "Ground")
                    _commandProcessor.ExecuteCommand(new MoveCommand(gameObject, _mouseInput.GetMovementPosition()));
                else if (_objectHit.transform.tag == "Enemy")
                    _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsTargeting"));
            }

            if (Input.GetKeyDown(KeyCode.D))
                _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsAutoAttacking"));

            if (Input.GetKeyDown(KeyCode.C))
                ChangeCloak();

            if (Input.GetKey(KeyCode.F))
            {
                if (_characterEquipment.ActiveEquipment != null)
                {
                    _commandProcessor.ExecuteCommand(new RotateToMouseCommand(transform));
                    _commandProcessor.ExecuteCommand(new UtiliseCommand(_characterEquipment, netId));
                }
                else
                    Debug.Log("You are not holding anything that is useable!");
            }

            if (Input.GetKeyDown(KeyCode.Q))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(_characterEquipment, 1));

            if (Input.GetKeyDown(KeyCode.W))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(_characterEquipment, 2));

            if (Input.GetKeyDown(KeyCode.E))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(_characterEquipment, 3));
            
            if (Input.GetKeyDown(KeyCode.R))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(_characterEquipment, 4));
        }

        if (Input.GetKeyDown(KeyCode.G))
            _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsQueueingCommands"));

        if (Input.GetKeyDown(KeyCode.Alpha1))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(1));

        if (Input.GetKeyDown(KeyCode.Alpha2))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(2));

        if (Input.GetKeyDown(KeyCode.Alpha3))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(3));

        if (Input.GetKeyDown(KeyCode.Alpha4))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(4));

        if (Input.GetKeyDown(KeyCode.Tab))
            _commandProcessor.ExecuteCommand(new ToggleScoreboardCommand(true));
        if (Input.GetKeyUp(KeyCode.Tab))
            _commandProcessor.ExecuteCommand(new ToggleScoreboardCommand(false));

        if (IsAutoAttacking)
        {
            if (_characterEquipment.ActiveEquipment is IWeapon)
                _commandProcessor.ExecuteCommand(new AutoAttackCommand(gameObject));
        }

        if (IsTargeting)
        {
            _commandProcessor.ExecuteCommand(new TargetCommand(gameObject, _objectHit.transform, _characterEquipment));

            if (_objectHit.transform == null)
                _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsTargeting"));
        }

        if (IsCloaked && NetworkTime.time - _lastCloakCostTime >= 3)
        {
            _commandProcessor.ExecuteCommand(new MaintainCloakCommand(this, netId));
            _lastCloakCostTime = NetworkTime.time;
        }
    
    }

    public void ChangeCloak()
    {
        _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsCloaked"));
        _commandProcessor.ExecuteCommand(new CloakCommand(IsCloaked, netId));
        
        _lastCloakCostTime = NetworkTime.time;
    }
}
