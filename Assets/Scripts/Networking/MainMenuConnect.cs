using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuConnect : MonoBehaviour
{
    [SerializeField] private MyNetworkManager _manager;
    
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
