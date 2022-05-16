using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class MainMenuConnect : MonoBehaviour
{
    public TMP_InputField PlayerNameInput;
    private MyNetworkManager _manager;

    private void Awake()
    {
        _manager = GetComponent<MyNetworkManager>();
    }
    
    public void OnHostGameBtnClick()
    {
        if (PlayerNameInput.text == "")
        {
            Debug.Log("Please enter your player name");
            return;
        }

        _manager.StartHost();
    }

    public void OnJoinGameBtnClick()
    {
        if (PlayerNameInput.text == "")
        {
            Debug.Log("Please enter your player name");
            return;
        }

        _manager.StartClient();
    }

    // might take this option out depending on if it's buggy or not
    public void OnStartServerBtnClick()
    {
        _manager.StartServer();
    }
}
