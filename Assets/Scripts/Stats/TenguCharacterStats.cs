using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenguCharacterStats : CharacterStats
{
    public override Stat Defence { get; } = new Stat("Defence", 5);
    public override Stat Speed { get; } = new Stat("Speed", 5);
}
