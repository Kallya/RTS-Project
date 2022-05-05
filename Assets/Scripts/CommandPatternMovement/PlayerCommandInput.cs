using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MouseClickInput))]
public class PlayerCommandInput : NetworkBehaviour
{
    public bool IsQueueingCommands { get; set; } = false;
    public bool IsAutoAttacking { get; set; } = false;
    public bool IsCloaked { get; set; } = false;
    public bool IsTargeting { get; set; } = false;

    private CommandProcessor _commandProcessor;
    private MouseClickInput _mouseInput;
    private PlayerEquipment _playerEquipment;
    private RaycastHit _objectHit;

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
        _playerEquipment = GetComponent<PlayerEquipment>();
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
                _commandProcessor.QueueCommand(new ChangeToggleCommand(this, "IsCloaked"));

            if (Input.GetKey(KeyCode.F) && _playerEquipment.ActiveEquipment != null)
            {
                if (_playerEquipment.ActiveEquipment != null)
                {
                    if (_playerEquipment.ActiveEquipment is IWeapon)
                        _commandProcessor.QueueCommand(new AttackCommand(gameObject));
                    else
                        _commandProcessor.QueueCommand(new ActivateCommand(gameObject));
                }
                else
                    Debug.Log("You are not holding anything that is useable!");
            }

            if (Input.GetKeyDown(KeyCode.Q))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(gameObject, 1));

            if (Input.GetKeyDown(KeyCode.W))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(gameObject, 2));

            if (Input.GetKeyDown(KeyCode.E))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(gameObject, 3));

            if (Input.GetKeyDown(KeyCode.R))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(gameObject, 4));
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
                _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsCloaked"));

            if (Input.GetKey(KeyCode.F))
            {
                if (_playerEquipment.ActiveEquipment != null)
                {
                    _commandProcessor.ExecuteCommand(new RotateToMouseCommand(gameObject));
                    if (_playerEquipment.ActiveEquipment is IWeapon)
                        _commandProcessor.ExecuteCommand(new AttackCommand(gameObject));
                    else
                        _commandProcessor.ExecuteCommand(new ActivateCommand(gameObject));
                }
                else
                    Debug.Log("You are not holding anything that is useable!");
            }

            if (Input.GetKeyDown(KeyCode.Q))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(gameObject, 1));

            if (Input.GetKeyDown(KeyCode.W))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(gameObject, 2));

            if (Input.GetKeyDown(KeyCode.E))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(gameObject, 3));
            
            if (Input.GetKeyDown(KeyCode.R))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(gameObject, 4));
        }

        if (Input.GetKeyDown(KeyCode.G))
            _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsQueueingCommands"));

        if (Input.GetKeyDown(KeyCode.Alpha1))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 1));

        if (Input.GetKeyDown(KeyCode.Alpha2))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 2));

        if (Input.GetKeyDown(KeyCode.Alpha3))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 3));

        if (Input.GetKeyDown(KeyCode.Alpha4))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 4));

        if (Input.GetKeyDown(KeyCode.Tab))
            _commandProcessor.ExecuteCommand(new ToggleScoreboardCommand(true));
        if (Input.GetKeyUp(KeyCode.Tab))
            _commandProcessor.ExecuteCommand(new ToggleScoreboardCommand(false));

        if (IsAutoAttacking)
        {
            if (_playerEquipment.ActiveEquipment is IWeapon)
                _commandProcessor.ExecuteCommand(new AutoAttackCommand(gameObject));
        }

        if (IsTargeting)
        {
            _commandProcessor.ExecuteCommand(new TargetCommand(gameObject, _objectHit.transform));

            if (_objectHit.transform == null)
                _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsTargeting"));
        }
    }
}
