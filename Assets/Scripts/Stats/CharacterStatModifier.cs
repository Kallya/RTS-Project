using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatModifier : MonoBehaviour
{
    public CharacterStatModifier Instance { get; private set; }

    private Dictionary<GameObject, CharacterStats> _characterStats;

    private void Awake()
    {
        Instance = this;
    }

    public void Setup()
    {
        foreach (GameObject localCharacter in POVManager.Instance.ActiveCharacters)
            _characterStats.Add(localCharacter, localCharacter.GetComponent<CharacterStats>());
    }

    public void ModifyCharacterStat(GameObject character, string stat, int value)
    {
        CharacterStats stats = _characterStats[character];
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
    }
}
