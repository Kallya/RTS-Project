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
    public CinemachineVirtualCamera CurrVirtualCam { get; private set; }
    public CinemachineFramingTransposer CurrVirtualCamBody { get; private set; }
    public Collider BoundingCollider;

    private List<CharacterCommandInput> _playerInputs = new List<CharacterCommandInput>();
    private Dictionary<GameObject, PlayerSpriteReferences> _spriteReferences = new Dictionary<GameObject, PlayerSpriteReferences>();
    private List<CinemachineVirtualCamera> _characterCams = new List<CinemachineVirtualCamera>();
    private List<CinemachineFramingTransposer> _characterCamBodies = new List<CinemachineFramingTransposer>();
    [SerializeField] private GameObject _characterVirtualCamPrefab;
    [SerializeField] private GameObject _friendlySprite;
    [SerializeField] private GameObject _enemySprite;
    [SerializeField] private GameObject _rangeIndicatorSprite;
    [SerializeField] private GameObject _allyHealthBarSprite;
    [SerializeField] private GameObject _enemyHealthBarSprite;
    [SerializeField] private GameObject _statBarCanvas;
    private static string _minimapSpriteName = "MinimapSprite";
    private static int _cloakedLayer = 7;
    private static int _minimapLayer = 6;
    private static Color normalMinimapSpriteColour = new Color(0f, 1f, 0f, .5f);
    private static Color selectedMinimapSpriteColour = Color.cyan;

    private void Awake()
    {
        Instance = this;
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
                AssignCamera(character); // create and assign individual camera to character for switching POVs
                ActiveCharacters.Add(character);
                AddMinimapSprite(character, false);
                AddRangeIndicator(character);
                AddCharacterStatBars(character, false);
            }
            else
            {
                character.tag = "Enemy"; // differentiate enemy characters
                AddMinimapSprite(character, true);
                AddCharacterStatBars(character, true);
            }
        }
        
        // Get all ally characters' input controllers
        foreach (GameObject character in ActiveCharacters)
        {
            _playerInputs.Add(character.GetComponent<CharacterCommandInput>());
            character.GetComponent<DamageableCharacter>().OnDestroyed += Destroyed; // trigger for auto pov change
        }

        ChangePOV(1);
    }

    private void Destroyed(GameObject character)
    {
        // ignore auto POV switch to active character
        // if destroyed character isn't current character
        if (character.transform != CurrVirtualCam.Follow)
            return; 

        for (int i = 0; i < ActiveCharacters.Count; i++)
        {
            GameObject nextCharacter = ActiveCharacters[i];
            if (character != null && nextCharacter != character)
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

        // prevent pov changes to dead characters
        if (ActiveCharacters[characterIndex] == null)
            return;

        float zoomDist = 30f; // default zoom value

        if (CurrVirtualCam != null)
        {
            _playerInputs[characterIndex].enabled = false;
            _spriteReferences[CurrVirtualCam.Follow.gameObject].RangeIndicatorSprite.SetActive(false);
            _spriteReferences[CurrVirtualCam.Follow.gameObject].MinimapSpriteRenderer.color = normalMinimapSpriteColour;
            // CurrVirtualCam.Follow.GetComponent<CharacterCommandInput>().enabled = false;
            // CurrVirtualCam.Follow.GetComponent<PlayerSpriteReferences>().RangeIndicatorSprite.SetActive(false);
            zoomDist = CurrVirtualCamBody.m_CameraDistance;
        }

        CurrVirtualCam = _characterCams[characterIndex];
        CurrVirtualCamBody = _characterCamBodies[characterIndex];

        _playerInputs[characterIndex].enabled = true;
        CurrVirtualCamBody.m_TrackedObjectOffset = Vector3.zero; // center cam on new character
        CurrVirtualCamBody.m_CameraDistance = zoomDist; // transfer previous zoom value
        CurrVirtualCam.MoveToTopOfPrioritySubqueue(); // switch active camera

        _spriteReferences[ActiveCharacters[characterIndex]].RangeIndicatorSprite.SetActive(true);
        // differentiate current character from others to help locate on minimap
        _spriteReferences[CurrVirtualCam.Follow.gameObject].MinimapSpriteRenderer.color = selectedMinimapSpriteColour;
        
        OnPOVChanged?.Invoke(CurrVirtualCam.Follow);
    }

    private void AssignCamera(GameObject character)
    {
        GameObject cam = Instantiate(_characterVirtualCamPrefab);
        CinemachineVirtualCamera camComponent = cam.GetComponent<CinemachineVirtualCamera>();

        camComponent.Follow = character.transform;
        cam.GetComponent<CinemachineConfiner>().m_BoundingVolume = BoundingCollider; // to confine to edges of map

        _characterCams.Add(camComponent);
        _characterCamBodies.Add(camComponent.GetCinemachineComponent<CinemachineFramingTransposer>());
    }

    // add minimap sprite to character identifying allies and enemies
    private void AddMinimapSprite(GameObject character, bool isEnemy)
    {
        GameObject minimapSprite;

        if (isEnemy == true)
            minimapSprite = Instantiate(_enemySprite, character.transform);
        else
            minimapSprite = Instantiate(_friendlySprite, character.transform);

        minimapSprite.name = _minimapSpriteName;
        _spriteReferences[character].MinimapSprite = minimapSprite;
        _spriteReferences[character].MinimapSpriteRenderer = minimapSprite.GetComponent<SpriteRenderer>();
        _spriteReferences[character].MinimapSpriteRenderer.color = normalMinimapSpriteColour;
    }

    private void AddRangeIndicator(GameObject character)
    {
        GameObject indicator = Instantiate(_rangeIndicatorSprite, character.transform);
        indicator.SetActive(false);
        _spriteReferences[character].RangeIndicatorSprite = indicator;

        character.GetComponent<CharacterEquipment>().RangeIndicatorSprite = indicator;
    }

    private void AddCharacterStatBars(GameObject character, bool isEnemy)
    {
        GameObject statBarCanvas = Instantiate(_statBarCanvas, character.transform);
        
        if (isEnemy == true)
            Instantiate(_enemyHealthBarSprite, statBarCanvas.transform);
        else
            Instantiate(_allyHealthBarSprite, statBarCanvas.transform);
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
