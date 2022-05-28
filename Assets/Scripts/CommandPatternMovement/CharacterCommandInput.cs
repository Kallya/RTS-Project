using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class CharacterCommandInput : NetworkBehaviour
{
    public static bool InputsEnabled { get; set; } = false; // global scope

    public bool IsQueueingCommands { get; set; } = false;
    public bool IsAutoAttacking { get; set; } = false;
    public bool IsCloaked { get; set; } = false;
    public bool IsTargeting { get; set; } = false;
    public double LastCloakCostTime { get; set; }

    private CommandProcessor _commandProcessor;
    private CharacterEquipment _characterEquipment;
    private PlayerSettings _playerSettings;
    private RaycastHit _objectHit;
    private Transform _lastTarget;
    private static int _cloakInterval = 3; // interval between cloak cost reduction

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
        _characterEquipment = GetComponent<CharacterEquipment>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Don't allow sole server to interact with game through inputs
        if (isServerOnly)
            return;

        // enable and disable inputs when transitioning between scenes
        // to allow leeway for things to load
        if (InputsEnabled == false)
            return;
        
        if (IsQueueingCommands)
        {
            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Move"]))
            {
                _objectHit = MouseClickInput.GetObjectHit(Camera.main);
                
                if (_objectHit.transform != null)
                {
                    if (_objectHit.transform.tag == "Ground")
                        _commandProcessor.QueueCommand(new MoveCommand(
                            gameObject, 
                            MouseClickInput.GetMovementPosition(transform, Camera.main
                        )));
                    else if (_objectHit.transform.tag == "Enemy")
                        _commandProcessor.QueueCommand(new ChangeToggleCommand(this, "IsTargeting"));
                }
            }
                
            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Undo"])) 
                _commandProcessor.Undo();

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Toggle Auto Attack"]))
                _commandProcessor.QueueCommand(new ChangeToggleCommand(this, "IsAutoAttacking"));

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Toggle Cloak"]))
                ChangeCloak();

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Utilise Equipment"]))
            {
                // only utilities are able to be queued (e.g. queueing a single gunshot or sword hit without aim seems useless)
                if (_characterEquipment.ActiveEquipment is IUtility)
                    _commandProcessor.QueueCommand(new UtiliseCommand(_characterEquipment, netId));
                else
                    Debug.Log("You are not holding anything that is useable!");
            }

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 1"]))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(_characterEquipment, 1));

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 2"]))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(_characterEquipment, 2));

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 3"]))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(_characterEquipment, 3));

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 4"]))
                _commandProcessor.QueueCommand(new SwitchWeaponCommand(_characterEquipment, 4));
        }
        else
        {
            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Move"]))
            {
                _objectHit = MouseClickInput.GetObjectHit(Camera.main);

                if (_objectHit.transform != null)
                {
                    if (_objectHit.transform.tag == "Ground")
                        _commandProcessor.ExecuteCommand(
                            new MoveCommand(
                                gameObject, 
                                MouseClickInput.GetMovementPosition(transform, Camera.main)
                            )
                        );
                    else if (_objectHit.transform.tag == "Enemy")
                        _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsTargeting"));
                }
            }

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Toggle Auto Attack"]))
                _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsAutoAttacking"));

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Toggle Cloak"]))
                ChangeCloak();

            if (Input.GetKey(PlayerSettings.s_HotkeyMappings["Utilise Equipment"]))
            {
                _commandProcessor.ExecuteCommand(new RotateToMouseCommand(transform));
                _commandProcessor.ExecuteCommand(new UtiliseCommand(_characterEquipment, netId));
            }

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 1"]))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(_characterEquipment, 1));

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 2"]))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(_characterEquipment, 2));

            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 3"]))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(_characterEquipment, 3));
            
            if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 4"]))
                _commandProcessor.ExecuteCommand(new SwitchWeaponCommand(_characterEquipment, 4));
        }

        // unqueueable commands
        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Toggle Queue Commands"]))
            _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsQueueingCommands"));

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Character 1"]))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(1));

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Character 2"]))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(2));

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Character 3"]))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(3));

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Character 4"]))
            _commandProcessor.ExecuteCommand(new ChangePOVCommand(4));

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Toggle Scoreboard"]))
            _commandProcessor.ExecuteCommand(new ToggleScoreboardCommand(true));
        if (Input.GetKeyUp(PlayerSettings.s_HotkeyMappings["Toggle Scoreboard"]))
            _commandProcessor.ExecuteCommand(new ToggleScoreboardCommand(false));

        // implementation of active toggles
        if (IsAutoAttacking)
        {
            if (_characterEquipment.ActiveEquipment is IWeapon)
                _commandProcessor.ExecuteCommand(new AutoAttackCommand(gameObject, netId));
        }

        if (IsTargeting)
        {
            _commandProcessor.ExecuteCommand(new TargetCommand(gameObject, _objectHit.transform, _characterEquipment, _commandProcessor));

            if (_objectHit.transform == null)
                _commandProcessor.ExecuteCommand(new ChangeToggleCommand(this, "IsTargeting"));
        }

        if (IsCloaked && NetworkTime.time - LastCloakCostTime >= _cloakInterval)
            _commandProcessor.ExecuteCommand(new MaintainCloakCommand(this, netId));
    
    }

    public void ChangeCloak()
    {
        ChangeToggleCommand toggleCmd = new ChangeToggleCommand(this, "IsCloaked");
        CloakCommand cloakCmd = new CloakCommand(this, IsCloaked, netId);
        if (IsQueueingCommands)
        {
            _commandProcessor.QueueCommand(toggleCmd);
            _commandProcessor.QueueCommand(cloakCmd);
        }
        else
        {
            _commandProcessor.ExecuteCommand(toggleCmd);
            _commandProcessor.ExecuteCommand(cloakCmd);
        }
    }
}
