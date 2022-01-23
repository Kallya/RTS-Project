using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public static class Stats
{
    // create scriptable object for storing default values?

    public static void IncreaseStat(Stat statToChange, int value)
    {
        statToChange.GetType().GetProperty("Value").SetValue(statToChange, statToChange.Value + value);
    }

    public static void DecreaseStat(Stat statToChange, int value)
    {
        statToChange.GetType().GetProperty("Value").SetValue(statToChange, statToChange.Value - value);
    }
}

public class Stat
{
    public event System.Action<Stat> OnStatChanged;
    public string Name { get; private set; }
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            OnStatChanged?.Invoke(this);
        }
    }
    public int BaseValue { get; private set; }

    // default constructor since deserialisation requires
    // though this isn't actually used
    public Stat() {}

    private int _value;

    public Stat(string name, int baseValue)
    {
        Name = name;
        BaseValue = baseValue;
        Value = baseValue;
    }
}
