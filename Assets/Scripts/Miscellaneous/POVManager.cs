using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using Mirror;

public class POVManager : NetworkBehaviour
{
    public static POVManager Instance { get; private set; }
    public event System.Action<Transform> OnPOVChanged;

    private List<GameObject> _activeCharacters = new List<GameObject>();
    private List<PlayerCommandInput> _playerInputs = new List<PlayerCommandInput>();
    private CinemachineVirtualCamera _vc;
    private CinemachineFramingTransposer _vcBody;

    private void Awake()
    {
        Instance = this;
        
        _vc = GetComponent<CinemachineVirtualCamera>();
        _vcBody = _vc.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    // Get all local characters (characters this client controls)
    public void SetLocalCharacters()
    {
        // Get all characters controllable by client
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<NetworkIdentity>().hasAuthority)
                _activeCharacters.Add(player);
            else
                player.tag = "Enemy"; // differentiate enemy characters
        }
        
        for (int i = 0; i < _activeCharacters.Count; i++)
            _playerInputs.Add(_activeCharacters[i].GetComponent<PlayerCommandInput>());

        ChangePOV(1);
    }

    public void ChangePOV(int characterNum)
    {
        if (characterNum > _activeCharacters.Count)
            return;
            
        int characterIndex = characterNum - 1;
            
        _playerInputs[characterIndex].enabled = true;
        _vc.Follow = _activeCharacters[characterIndex].transform;
        _vcBody.m_ScreenX = _vcBody.m_ScreenY = 0.5f;
        
        OnPOVChanged?.Invoke(_vc.Follow);
    }
}
