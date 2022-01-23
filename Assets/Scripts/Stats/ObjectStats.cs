using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObjectStats : DamageableObjectStats
{
    public override Stat Health { get; } = new Stat("Health", 200);

    private void Start()
    {
        Health.Value = Health.BaseValue;
    }
}
