using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : DamageableObject
{
    public Enemy() : base()
    {
        this.health = 1000;
    }

    public override void DestroyObject()
    {
        base.DestroyObject();
    }
}
