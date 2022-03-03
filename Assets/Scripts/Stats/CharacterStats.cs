using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CharacterStats : DamageableObjectStats
{
    public Sprite CharacterSprite;
    public Stat[] Stats;

    public override Stat Health { get; } = new Stat("Health", 100);
    public Stat Energy { get; } = new Stat("Energy", 100);
    public Stat Speed { get; } = new Stat("Speed", 5);
    public Stat Defence { get; } = new Stat("Defence", 5);

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
