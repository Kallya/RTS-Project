using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using Mirror;

public class ChangePOVListener : MonoBehaviour
{
    public static ChangePOVListener Instance { get; private set; }
    
    // this only be called once when a new player spawns
    public int LocalPlayerConnId
    {
        get => _localPlayerConnId;
        set
        {
            _localPlayerConnId = value;
            SetLocalPlayers();
        }
    }

    private int _localPlayerConnId;
    private GameObject[] _activeCharacters;
    private List<PlayerCommandInput> _playerInputs = new List<PlayerCommandInput>();
    private CinemachineVirtualCamera _vc;

    private void Awake()
    {
        Instance = this;
        
        _vc = GetComponent<CinemachineVirtualCamera>();
    }

    private void SetLocalPlayers()
    {
        _activeCharacters = GameObject.FindGameObjectsWithTag("Player")
            .Where(player => player.GetComponent<NetworkIdentity>().connectionToClient.connectionId == LocalPlayerConnId)
            .ToArray();
        
        for (int i = 0; i < _activeCharacters.Length; i++)
            _playerInputs.Add(_activeCharacters[i].GetComponent<PlayerCommandInput>());

        ChangePOV(1);
    }

    public void ChangePOV(int characterNum)
    {
        int characterIndex = characterNum - 1;
        _playerInputs[characterIndex].enabled = true;
        _vc.Follow = _activeCharacters[characterIndex].transform;
    }
}
