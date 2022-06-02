using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class MainMenuConnect : MonoBehaviour
{
    public TMP_InputField PlayerNameInput;

    private MyNetworkManager _manager;
    [SerializeField] private GameObject _connectingText;
    [SerializeField] private int _connectWaitTime = 3;
    [SerializeField] private CanvasGroup _menuCanvasGroup;
    [SerializeField] private GameObject _clientConnectUI;
    [SerializeField] private TMP_Text _connectIPAddress;

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

        _manager.StartHost();
    }

    public void JoinGameBtnClick()
    {
        if (PlayerNameInput.text == "")
        {
            ErrorNotifier.Instance.GenerateErrorMsg(ErrorNotifier.ErrorMessages[ErrorMessageType.NoPlayerName]);
            return;
        }

        _clientConnectUI.SetActive(true);
    }

    public void AttemptClientConnectBtnClick()
    {
        StartCoroutine(AttemptClientConnect());
    }

    private IEnumerator AttemptClientConnect()
    {
        // _manager.networkAddress = _connectIPAddress.text; // set to host's ip address (which should be entered)
        _manager.StartClient();
        _connectingText.SetActive(true);
        _menuCanvasGroup.interactable = false;

        yield return new WaitForSeconds(_connectWaitTime);

        if (NetworkClient.isConnected == false)
        {
            _manager.StopClient();
            _connectingText.SetActive(false);
            _menuCanvasGroup.interactable = true;
            ErrorNotifier.Instance.GenerateErrorMsg(ErrorNotifier.ErrorMessages[ErrorMessageType.NoServerActive]);
        }
    }
}
