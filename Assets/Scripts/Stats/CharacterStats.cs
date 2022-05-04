using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// base class for the stats of the three different character types
public abstract class CharacterStats : DamageableObjectStats
{
    public Sprite CharacterSprite;
    public Stat[] Stats;


    public override Stat Health { get; } = new Stat("Health", 100);
    public Stat Energy { get; } = new Stat("Energy", 100);
    public abstract Stat Speed { get; }
    public abstract Stat Defence { get; }

    private void Awake()
    {
        Stats = new Stat[] {Health, Energy, Speed, Defence};
    }

    private void Start()
    {
        InitialiseStats();
    }

    // reinitialise stats to trigger UI update
    public void InitialiseStats()
    {
        foreach (Stat stat in Stats)
            stat.Value = stat.Value;
    }
}
