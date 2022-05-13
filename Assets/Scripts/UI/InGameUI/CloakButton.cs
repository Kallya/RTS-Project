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

    private void Awake()
    {
        _cloakBtn = GetComponent<Image>();
        _cloakBtn.color = Color.white;
    }

    private void Start()
    {
        PlayerInfoUIManager.Instance.OnPOVChanged += POVChanged;
    }

    public void OnCloakBtnClick()
    {
        PlayerInfoUIManager.Instance.CurrCmdInput.ChangeCloak();
    }

    private void POVChanged()
    {
        SetColour(PlayerInfoUIManager.Instance.CurrCmdInput.IsCloaked);
    }

    private void SetColour(bool IsCloaked)
    {
        if (IsCloaked == true)
            _cloakBtn.color = Color.green;
        else
            _cloakBtn.color = Color.white;
    }
}
