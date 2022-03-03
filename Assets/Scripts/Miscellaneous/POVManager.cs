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
    public List<GameObject> ActiveCharacters = new List<GameObject>();

    private List<PlayerCommandInput> _playerInputs = new List<PlayerCommandInput>();
    private Dictionary<GameObject, PlayerSpriteReferences> _spriteReferences = new Dictionary<GameObject, PlayerSpriteReferences>();
    private CinemachineVirtualCamera _vc;
    private CinemachineFramingTransposer _vcBody;
    [SerializeField] private GameObject _friendlySprite;
    [SerializeField] private GameObject _enemySprite;
    [SerializeField] private GameObject _rangeIndicatorSprite;
    [SerializeField] private GameObject _allyHealthBarSprite;
    [SerializeField] private GameObject _enemyHealthBarSprite;
    [SerializeField] private GameObject _energyBar;
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
            _spriteReferences.Add(character, character.GetComponent<PlayerSpriteReferences>()); // cache sprite references

            // all of local client's characters have authority
            if (character.GetComponent<NetworkIdentity>().hasAuthority)
            {
                ActiveCharacters.Add(character);
                AddMinimapSprite(character, false);
                AddRangeIndicator(character);
                AddHealthBar(character, false);
                AddEnergyBar(character);
            }
            else
            {
                character.tag = "Enemy"; // differentiate enemy characters
                AddMinimapSprite(character, true);
                AddHealthBar(character, true);
                AddEnergyBar(character);
            }
        }
        
        // Get all ally characters' input controllers
        foreach (GameObject character in ActiveCharacters)
        {
            _playerInputs.Add(character.GetComponent<PlayerCommandInput>());
            character.GetComponent<DamageableCharacter>().OnDestroyed += Destroyed; // trigger for auto pov change
        }

        ChangePOV(1);
    }

    // automatically switch to active character POV when current one dies
    private void Destroyed(GameObject go)
    {
        if (go.transform != _vc.Follow)
            return; 

        for (int i = 0; i < ActiveCharacters.Count; i++)
        {
            GameObject character = ActiveCharacters[i];
            if (character != null && character != go)
            {
                ChangePOV(i+1);
                break;
            }
        }
    }

    public void ChangePOV(int characterNum)
    {
        if (characterNum > ActiveCharacters.Count)
            return;
            
        int characterIndex = characterNum - 1;

        // dead characters will be nulled to maintain
        // hotkeys for switching characters
        // this prevents pov changes to null objects
        if (ActiveCharacters[characterIndex] == null)
            return;

        if (_vc.Follow != null)
        {
            _vc.Follow.GetComponent<PlayerCommandInput>().enabled = false;
            _vc.Follow.GetComponent<PlayerSpriteReferences>().RangeIndicatorSprite.SetActive(false);
        }

        _playerInputs[characterIndex].enabled = true;
        _vc.Follow = ActiveCharacters[characterIndex].transform;
        _vcBody.m_TrackedObjectOffset = Vector3.zero;   // center cam on new character

        _spriteReferences[ActiveCharacters[characterIndex]].RangeIndicatorSprite.SetActive(true);
        
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
        _spriteReferences[character].MinimapSprite = minimapSprite;
    }

    private void AddRangeIndicator(GameObject character)
    {
        GameObject indicator = Instantiate(_rangeIndicatorSprite, character.transform);
        indicator.SetActive(false);
        _spriteReferences[character].RangeIndicatorSprite = indicator;

        character.GetComponent<PlayerEquipment>().RangeIndicatorSprite = indicator;
    }

    private void AddHealthBar(GameObject character, bool isEnemy)
    {
        if (isEnemy == true)
            Instantiate(_enemyHealthBarSprite, character.transform);
        else
            Instantiate(_allyHealthBarSprite, character.transform);
    }

    private void AddEnergyBar(GameObject character)
    {
        Instantiate(_energyBar, character.transform);
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

        GameObject minimapSprite = _spriteReferences[targetCharacter.gameObject].MinimapSprite;

        // set layers so minimap cam will or will not render
        if (isCloaked == true)
            minimapSprite.layer = _cloakedLayer;
        else
            minimapSprite.layer = _minimapLayer;

    }
}
