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

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Move"]))
        {
            _objectHit = MouseClickInput.GetObjectHit(Camera.main);

            if (_objectHit.transform != null)
            {
                if (_objectHit.transform.tag == "Ground")
                    ExecuteQueueableCmd(new MoveCommand(gameObject, MouseClickInput.GetMovementPosition(transform, Camera.main)));
                else if (_objectHit.transform.tag == "Enemy")
                    ExecuteQueueableCmd(new ChangeToggleCommand(this, "IsTargeting"));
            }
        }

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Toggle Auto Attack"]))
            ExecuteQueueableCmd(new ChangeToggleCommand(this, "IsAutoAttacking"));

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Toggle Cloak"]))
            ChangeCloak();

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Utilise Equipment"]) && IsQueueingCommands == true)
        {
            // only utilities are able to be queued (e.g. queueing a single gunshot or sword hit without aim seems useless)
            if (_characterEquipment.ActiveEquipment is IUtility)
                ExecuteQueueableCmd(new UtiliseCommand(_characterEquipment, netId));
            else
                ErrorNotifier.Instance.GenerateErrorMsg(ErrorNotifier.ErrorMessages[ErrorMessageType.NotHoldingQueueableEquipment]);
        }

        if (Input.GetKey(PlayerSettings.s_HotkeyMappings["Utilise Equipment"]) && IsQueueingCommands == false)
        {
            _commandProcessor.ExecuteCommand(new RotateToMouseCommand(transform));
            _commandProcessor.ExecuteCommand(new UtiliseCommand(_characterEquipment, netId));
        }

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Bail Out"]))
            ExecuteQueueableCmd(new BailOutCommand(gameObject));

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 1"]))
            ExecuteQueueableCmd(new SwitchWeaponCommand(_characterEquipment, 1));

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 2"]))
            ExecuteQueueableCmd(new SwitchWeaponCommand(_characterEquipment, 2));

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 3"]))
            ExecuteQueueableCmd(new SwitchWeaponCommand(_characterEquipment, 3));

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Switch to Equipment 4"]))
            ExecuteQueueableCmd(new SwitchWeaponCommand(_characterEquipment, 4));

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

        if (Input.GetKeyDown(PlayerSettings.s_HotkeyMappings["Undo"]))
            _commandProcessor.Undo();

        // implementation of active toggles
        if (IsAutoAttacking)
        {
            if (_characterEquipment.ActiveEquipment is IWeapon)
                _commandProcessor.ExecuteCommand(new AutoAttackCommand(gameObject, netId));
        }

        if (IsTargeting)
        {
            _commandProcessor.ExecuteCommand(new TargetCommand(gameObject, _objectHit.transform, _characterEquipment, _commandProcessor));

            if (_objectHit.transform.tag != "Enemy")
                IsTargeting = false;
        }

        if (IsCloaked && NetworkTime.time - LastCloakCostTime >= _cloakInterval)
            _commandProcessor.ExecuteCommand(new MaintainCloakCommand(this, netId));
    
    }

    public void ChangeCloak()
    {
        ExecuteQueueableCmd(new ChangeToggleCommand(this, "IsCloaked"));
        ExecuteQueueableCmd(new CloakCommand(this, netId));
    }

    public void ExecuteQueueableCmd(IQueueableCommand cmd)
    {
        if (IsQueueingCommands)
            _commandProcessor.QueueCommand(cmd);
        else
            _commandProcessor.ExecuteCommand(cmd);
    }
}
