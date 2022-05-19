using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterStatModifier : NetworkBehaviour
{
    public static CharacterStatModifier Instance { get; private set; }

    private Dictionary<uint, CharacterStats> _characterStats = new Dictionary<uint, CharacterStats>();

    private void Awake()
    {
        Instance = this;
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
            _characterStats.Add(characterNetId, character.GetComponent<CharacterStats>());
            RpcAddCharacterStat(characterNetId);
        }
    }

    [ClientRpc]
    private void RpcAddCharacterStat(uint characterNetId)
    {
        if (isServer)
            return;

        GameObject character = NetworkClient.spawned[characterNetId].gameObject;
        _characterStats.Add(characterNetId, character.GetComponent<CharacterStats>());
    }

    private Stat GetCharacterStat(uint characterNetId, string stat)
    {
        CharacterStats stats = _characterStats[characterNetId];
        Stat statToModify = null;

        switch (stat)
        {
            case "Health":
                statToModify = stats.Health;
                break;
            case "Energy":
                statToModify = stats.Energy;
                break;
            case "Defence":
                statToModify = stats.Defence;
                break;
            case "Speed":
                statToModify = stats.Speed;
                break;
        }

        return statToModify;
    }

    private void SetCharacterStat(uint characterNetId, string stat, int value)
    {
        Stat statToSet = GetCharacterStat(characterNetId, stat);
        Stats.SetStat(statToSet, value);
    }

    [Command(requiresAuthority=false)]
    public void CmdSetCharacterStat(uint characterNetId, string stat, int value)
    {
        SetCharacterStat(characterNetId, stat, value);
        RpcSetCharacterStat(characterNetId, stat, value);
    }

    private void RpcSetCharacterStat(uint characterNetId, string stat, int value)
    {
        if (isServer)
            return;
        
        SetCharacterStat(characterNetId, stat, value);
    }

    private void DecreaseCharacterStat(uint characterNetId, string stat, int value)
    {
        Stat statToDecrease = GetCharacterStat(characterNetId, stat);
        Stats.DecreaseStat(statToDecrease, value);
    }

    [Command(requiresAuthority=false)]
    public void CmdDecreaseCharacterStat(uint characterNetId, string stat, int value)
    {
        DecreaseCharacterStat(characterNetId, stat, value);
        RpcDecreaseCharacterStat(characterNetId, stat, value);
    }

    [ClientRpc]
    public void RpcDecreaseCharacterStat(uint characterNetId, string stat, int value)
    {
        if (isServer)
            return;
            
        DecreaseCharacterStat(characterNetId, stat, value);
    }

    public bool CanDecreaseStat(uint characterNetId, string stat, int value)
    {
        Stat statToDecrease = GetCharacterStat(characterNetId, stat);

        if (statToDecrease.Value < value)
            return false;
        else
            return true;
    }

    private void IncreaseCharacterStat(uint characterNetId, string stat, int value)
    {
        Stat statToIncrease = GetCharacterStat(characterNetId, stat);
        Stats.IncreaseStat(statToIncrease, value);
    }

    [Command(requiresAuthority=false)]
    public void CmdIncreaseCharacterStat(uint characterNetId, string stat, int value)
    {
        IncreaseCharacterStat(characterNetId, stat, value);
        RpcIncreaseCharacterStat(characterNetId, stat, value);
    }

    [ClientRpc]
    public void RpcIncreaseCharacterStat(uint characterNetId, string stat, int value)
    {
        if (isServer)
            return;

        IncreaseCharacterStat(characterNetId, stat, value);
    }
}
