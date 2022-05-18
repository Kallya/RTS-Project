using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance { get; private set; }

    private Dictionary<uint, AudioSource> _characterAudioSources = new Dictionary<uint, AudioSource>();
    private List<AudioClip> _audioClips = new List<AudioClip>(); // assign in inspector
    private Dictionary<string, AudioClip> _audioClipsDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (AudioClip clip in _audioClips)
            _audioClipsDict.Add(clip.name, clip);
    }

    private void Setup()
    {
        if (!isServer)
            return;

        MyNetworkManager netManager = MyNetworkManager.singleton as MyNetworkManager;
        foreach (GameObject character in netManager.SpawnedCharacters)
        {
            uint characterNetId = character.GetComponent<NetworkIdentity>().netId;
        }
    }

    [Command(requiresAuthority=false)]
    private void PlayAudio(GameObject character, string clipName)
    {
        
    }
}