using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStats : DamageableObjectStats
{
    public override Stat Health { get; } = new Stat(200);
}
