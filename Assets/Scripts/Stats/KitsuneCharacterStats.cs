using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitsuneCharacterStats : CharacterStats
{
    public override Stat Defence { get; } = new Stat("Defence", 4);
    public override Stat Speed { get; } = new Stat("Speed", 10);
}
