using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenguCharacterStats : CharacterStats
{
    public override Stat Defence { get; } = new Stat("Defence", 7);
    public override Stat Speed { get; } = new Stat("Speed", 7);
}
