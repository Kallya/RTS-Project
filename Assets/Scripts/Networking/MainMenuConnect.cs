using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
            Debug.Log("Please enter your player name");
            return;
        }

        _manager.StartHost();
    }

    public void JoinGameBtnClick()
    {
        if (PlayerNameInput.text == "")
        {
            Debug.Log("Please enter your player name");
            return;
        }

        _manager.StartClient();
    }

/*
    // buggy
    public void OnStartServerBtnClick()
    {
        _manager.StartServer();
    }
*/
}
