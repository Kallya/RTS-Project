using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuConnect : MonoBehaviour
{
    public TMP_InputField PlayerNameInput;

    private MyNetworkManager _manager;

    private void Awake()
    {
        _manager = GetComponent<MyNetworkManager>();
    }
    
    public void HostGameBtnClick()
    {
        if (PlayerNameInput.text == "")
        {
            ErrorNotifier.Instance.GenerateErrorMsg(ErrorNotifier.ErrorMessages[ErrorMessageType.NoPlayerName]);
            return;
        }

        if (MyNetworkManager.ConnectedPlayerNames.Contains(PlayerNameInput.text))
        {
            ErrorNotifier.Instance.GenerateErrorMsg(ErrorNotifier.ErrorMessages[ErrorMessageType.UnavailablePlayerName]);
            return;
        }

        _manager.StartHost();

        MyNetworkManager.ConnectedPlayerNames.Add(PlayerNameInput.text);
    }

    public void JoinGameBtnClick()
    {
        if (PlayerNameInput.text == "")
        {
            ErrorNotifier.Instance.GenerateErrorMsg(ErrorNotifier.ErrorMessages[ErrorMessageType.NoPlayerName]);
            return;
        }

        if (MyNetworkManager.ConnectedPlayerNames.Contains(PlayerNameInput.text))
        {
            ErrorNotifier.Instance.GenerateErrorMsg(ErrorNotifier.ErrorMessages[ErrorMessageType.UnavailablePlayerName]);
            return;
        }

        _manager.StartClient();

        MyNetworkManager.ConnectedPlayerNames.Add(PlayerNameInput.text);
    }

/*
    // buggy
    public void OnStartServerBtnClick()
    {
        _manager.StartServer();
    }
*/
}
