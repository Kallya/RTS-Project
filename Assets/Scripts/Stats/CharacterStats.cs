using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterStats : DamageableObjectStats
{
    public Stat[] Stats;

    // synced fields
    public override Stat Health { get; } = new Stat("Health", 100);
    public Stat Energy { get; } = new Stat("Energy", 100);
    public Stat Speed { get; } = new Stat("Speed", 50);
    public Stat Defence { get; } = new Stat("Defence", 5);

    private void Awake()
    {
        Stats = new Stat[] {Health, Energy, Speed, Defence};
    }

    private void OnEnable()
    {
        // stat values reinitialised here to trigger stat changed event
        // so UI updates
        Health.Value = Health.Value;
        Energy.Value = Energy.Value;
        Speed.Value = Speed.Value;
        Defence.Value = Defence.Value;
    }
}
