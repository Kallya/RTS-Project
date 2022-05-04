using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public static class Stats
{
    public static void IncreaseStat(Stat statToChange, int value)
    {
        // statToChange.GetType().GetProperty("Value").SetValue(statToChange, statToChange.Value + value);

        statToChange.Value += value;
    }

    public static void DecreaseStat(Stat statToChange, int value)
    {
        // statToChange.GetType().GetProperty("Value").SetValue(statToChange, statToChange.Value - value);

        statToChange.Value -= value;
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

    private int _value;

    public Stat(string name, int baseValue)
    {
        Name = name;
        BaseValue = baseValue;
        Value = baseValue;
    }
}

public static class StatSerialiser
{
    public static void WriteStat(this NetworkWriter writer, Stat value)
    {
        writer.WriteInt(value.Value);
        writer.WriteString(value.Name);
    }

    public static Stat ReadStat(this NetworkReader reader)
    {
        return new Stat(reader.ReadString(), reader.ReadInt());
    }
}
