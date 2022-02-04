using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class DamageableObjectStats : MonoBehaviour
{
    public abstract Stat Health { get; }
}
