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
    [SerializeField] private GameObject _friendlySprite;
    [SerializeField] private GameObject _enemySprite;
    private static string _minimapSpriteName = "MinimapSprite";
    private static int _cloakedLayer = 7;
    private static int _minimapLayer = 6;

    private void Awake()
    {
        Instance = this;
        
        _vc = GetComponent<CinemachineVirtualCamera>();
        _vcBody = _vc.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<CloakMessage>(OnCloaked);
    }

    // setup allied and enemy characters
    public void SetLocalCharacters()
    {
        // Get all characters controllable by client
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Player"))
        {
            // all of local client's characters have authority
            if (character.GetComponent<NetworkIdentity>().hasAuthority)
            {
                _activeCharacters.Add(character);
                AddMinimapSprite(character, false);
            }
            else
            {
                character.tag = "Enemy"; // differentiate enemy characters
                AddMinimapSprite(character, true);
            }
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

        if (_vc.Follow != null)
            _vc.Follow.GetComponent<PlayerCommandInput>().enabled = false;

        _playerInputs[characterIndex].enabled = true;
        _vc.Follow = _activeCharacters[characterIndex].transform;
        _vcBody.m_ScreenX = _vcBody.m_ScreenY = 0.5f;
        
        OnPOVChanged?.Invoke(_vc.Follow);
    }

    private void AddMinimapSprite(GameObject character, bool isEnemy)
    {
        GameObject minimapSprite;

        if (isEnemy == true)
            minimapSprite = Instantiate(_enemySprite, character.transform);
        else
            minimapSprite = Instantiate(_friendlySprite, character.transform);

        minimapSprite.name = _minimapSpriteName;
    }

    private void OnCloaked(NetworkConnection conn, CloakMessage msg)
    {
        RpcSetTargetCloak(msg.CharacterNetId, msg.IsCloaked);
    }

    [ClientRpc]
    private void RpcSetTargetCloak(uint netId, bool isCloaked)
    {
        NetworkIdentity targetCharacter = NetworkClient.spawned[netId];

        // if target character is owned by local player (on their team), then cloak doesn't affect the client's minimap
        if (targetCharacter.hasAuthority == true)
            return;

        GameObject minimapSprite = targetCharacter.transform.Find(_minimapSpriteName).gameObject;

        // set layers so minimap cam will or will not render
        if (isCloaked == true)
            minimapSprite.layer = _cloakedLayer;
        else
            minimapSprite.layer = _minimapLayer;

    }
}
