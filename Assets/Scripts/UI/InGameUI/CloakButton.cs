using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public struct CloakMessage : NetworkMessage
{
    public uint CharacterNetId;
    public bool IsCloaked;
}
public class CloakButton : MonoBehaviour
{
    private Image _cloakBtn;
    private PlayerInfoUIManager _playerInfoUIManager;

    private void Awake()
    {
        _cloakBtn = GetComponent<Image>();
    }
    
    private void Start()
    {
        _playerInfoUIManager = PlayerInfoUIManager.Instance;
        _playerInfoUIManager.OnPOVChanged += POVChanged;
        _playerInfoUIManager.OnToggleChanged += ToggleChanged;
    }

    public void OnCloakBtnClick()
    {
        _playerInfoUIManager.CurrCmdInput.ChangeCloak();

    }

    private void ToggleChanged(string toggleName)
    {
        if (toggleName == "IsCloaked")
            SetColour(_playerInfoUIManager.CurrCmdInput.IsCloaked);
    }

    private void POVChanged()
    {
        SetColour(_playerInfoUIManager.CurrCmdInput.IsCloaked);
    }

    private void SetColour(bool IsCloaked)
    {
        if (IsCloaked == true)
            _cloakBtn.color = Color.green;
        else
            _cloakBtn.color = Color.white;
    }
}
