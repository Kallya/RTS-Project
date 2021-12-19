using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
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

public abstract class DamageableObjectStats : MonoBehaviour
{
    public abstract Stat Health { get; }
}

public class Stat
{
    public int Value { get; set; }

    public Stat(int baseValue)
    {
        Value = baseValue;
    }
}
