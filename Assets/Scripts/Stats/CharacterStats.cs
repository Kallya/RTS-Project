using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterStats : DamageableObjectStats
{
    public override Stat Health { get; } = new Stat(100);
    public Stat Energy { get; } = new Stat(100);
    public Stat Speed { get; } = new Stat(50);
}
