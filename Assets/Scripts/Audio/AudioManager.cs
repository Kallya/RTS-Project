using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance { get; private set; }

    private Dictionary<uint, AudioSource> _characterAudioSources = new Dictionary<uint, AudioSource>();
    [SerializeField] private List<AudioClip> _audioClips = new List<AudioClip>(); // assign in inspector
    private Dictionary<string, AudioClip> _audioClipsDict = new Dictionary<string, AudioClip>();
    private AudioSource _thisAudioSource;
    [SerializeField] private AudioClip _crunchTimeTrack;

    private void Awake()
    {
        Instance = this;

        _thisAudioSource = GetComponent<AudioSource>();

        foreach (AudioClip clip in _audioClips)
            _audioClipsDict.Add(clip.name, clip);
    }

    // setup performed via server
    // cause SpawnedCharacters is only updated on server
    [Server]
    public void Setup()
    {   
        MyNetworkManager netManager = MyNetworkManager.singleton as MyNetworkManager;
        foreach (GameObject character in netManager.SpawnedCharacters)
        {
            uint characterNetId = character.GetComponent<NetworkIdentity>().netId;
            _characterAudioSources.Add(characterNetId, character.GetComponent<AudioSource>());
            RpcAddAudioSource(characterNetId);
        }
    }

    private void Start()
    {
        MyNetworkManager netManager = MyNetworkManager.singleton as MyNetworkManager;
        
        Clock.Instance.OnCrunchTime += CrunchTime;
    }

    private void CrunchTime()
    {
        _thisAudioSource.clip = _crunchTimeTrack;
    }
    

    [ClientRpc]
    private void RpcAddAudioSource(uint characterNetId)
    {
        if (isServer)
            return;

        GameObject character = NetworkClient.spawned[characterNetId].gameObject;
        _characterAudioSources.Add(characterNetId, character.GetComponent<AudioSource>());
    }

    [Command(requiresAuthority=false)]
    public void CmdPlayAudio(uint characterNetId, string clipName)
    {
        PlayAudio(characterNetId, clipName);
        RpcPlayAudio(characterNetId, clipName);
    }

    private void PlayAudio(uint characterNetId, string clipName)
    {
        AudioSource audioSource = _characterAudioSources[characterNetId];
        audioSource.clip = _audioClipsDict[clipName];
        audioSource.Play();
    }

    [ClientRpc]
    private void RpcPlayAudio(uint characterNetId, string clipName)
    {
        if (isServer)
            return;

        PlayAudio(characterNetId, clipName);
    }
}