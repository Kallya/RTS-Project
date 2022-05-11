using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KabukiCharacterStats : CharacterStats
{
    public override Stat Defence { get; } = new Stat("Defence", 10);
    public override Stat Speed { get; } = new Stat("Speed", 4);
}
