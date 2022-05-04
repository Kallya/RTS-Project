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

    public void Setup()
    {
        if (!isServer)
            return;

        MyNetworkManager netManager = MyNetworkManager.singleton as MyNetworkManager;
        foreach (GameObject character in netManager.SpawnedCharacters)
        {
            uint characterNetId = character.GetComponent<NetworkIdentity>().netId;
            _characterStats.Add(characterNetId, character.GetComponent<CharacterStats>());
            RpcAddCharacterStat(characterNetId);
        }
    }

    [ClientRpc]
    private void RpcAddCharacterStat(uint netId)
    {
        if (isServer)
            return;

        GameObject character = NetworkClient.spawned[netId].gameObject;
        _characterStats.Add(netId, character.GetComponent<CharacterStats>());
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

    private void DecreaseCharacterStat(uint characterNetId, string stat, int value)
    {
        Stat statToDecrease = GetCharacterStat(characterNetId, stat);

        Stats.DecreaseStat(statToDecrease, value);
    }

    // return true or false based on if stat value changed or not
    [Command(requiresAuthority=false)]
    public void CmdDecreaseCharacterStat(uint characterNetId, string stat, int value)
    {
        DecreaseCharacterStat(characterNetId, stat, value);
        RpcDecreaseCharacterStat(characterNetId, stat, value);
    }

    [ClientRpc]
    private void RpcDecreaseCharacterStat(uint characterNetId, string stat, int value)
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

    public void IncreaseCharacterStat(uint characterNetId, string stat, int value)
    {
        Stat statToIncrease = GetCharacterStat(characterNetId, stat);
        Stats.IncreaseStat(statToIncrease, value);
    }
}
