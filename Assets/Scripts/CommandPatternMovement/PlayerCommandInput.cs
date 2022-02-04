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

    private CommandProcessor _commandProcessor;
    private MouseClickInput _mouseInput;

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
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsQueueingCommands)
        {
            if (Input.GetMouseButtonDown(1))
                _commandProcessor.QueueCommand(new MoveCommand(gameObject, _mouseInput.GetMovementPosition()));
                
            if (Input.GetKeyDown(KeyCode.T)) 
                _commandProcessor.Undo();

            if (Input.GetKeyDown(KeyCode.G))
                IsQueueingCommands = !IsQueueingCommands;

            if (Input.GetKeyDown(KeyCode.D))
                IsAutoAttacking = !IsAutoAttacking;

            if (Input.GetKeyDown(KeyCode.C))
                IsCloaked = !IsCloaked;

            if (Input.GetKey(KeyCode.F))
            {
                _commandProcessor.ExecuteCommand(new RotateToMouseCommand(gameObject));
                _commandProcessor.QueueCommand(new AttackCommand(gameObject));
            }

            if (Input.GetKeyDown(KeyCode.Q))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(gameObject, 1));

            if (Input.GetKeyDown(KeyCode.W))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(gameObject, 2));

            if (Input.GetKeyDown(KeyCode.E))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(gameObject, 3));

            if (Input.GetKeyDown(KeyCode.R))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(gameObject, 4));

            if (Input.GetKeyDown(KeyCode.Alpha1))
                _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 1));
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
                _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 2));

            if (Input.GetKeyDown(KeyCode.Alpha3))
                _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 3));

            if (Input.GetKeyDown(KeyCode.Alpha4))
                _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 4));
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
                _commandProcessor.ExecuteCommand(new MoveCommand(gameObject, _mouseInput.GetMovementPosition()));

            if (Input.GetKeyDown(KeyCode.G))
                IsQueueingCommands = !IsQueueingCommands;

            if (Input.GetKeyDown(KeyCode.D))
                IsAutoAttacking = !IsAutoAttacking;

            if (Input.GetKeyDown(KeyCode.C))
                IsCloaked = !IsCloaked;

            if (Input.GetKey(KeyCode.F))
            {
                _commandProcessor.ExecuteCommand(new RotateToMouseCommand(gameObject));
                _commandProcessor.ExecuteCommand(new AttackCommand(gameObject));
            }

            if (Input.GetKeyDown(KeyCode.Q))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(gameObject, 1));

            if (Input.GetKeyDown(KeyCode.W))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(gameObject, 2));

            if (Input.GetKeyDown(KeyCode.E))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(gameObject, 3));
            
            if (Input.GetKeyDown(KeyCode.R))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(gameObject, 4));

            if (Input.GetKeyDown(KeyCode.Alpha1))
                _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 1));
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
                _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 2));

            if (Input.GetKeyDown(KeyCode.Alpha3))
                _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 3));

            if (Input.GetKeyDown(KeyCode.Alpha4))
                _commandProcessor.ExecuteCommand(new ChangePOVCommand(gameObject, 4));
        }

        if (IsAutoAttacking)
            _commandProcessor.ExecuteCommand(new AutoAttackCommand(gameObject));
    }
}
