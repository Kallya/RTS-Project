using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class MainMenuConnect : MonoBehaviour
{
    public TMP_InputField _playerNameInput;
    private MyNetworkManager _manager;

    private void Awake()
    {
        _manager = GetComponent<MyNetworkManager>();
    }
    
    public void OnHostGameBtnClick()
    {
        _manager.StartHost();
    }

    public void OnJoinGameBtnClick()
    {
        _manager.StartClient();
    }

    public void OnStartServerBtnClick()
    {
        _manager.StartServer();
    }
}
