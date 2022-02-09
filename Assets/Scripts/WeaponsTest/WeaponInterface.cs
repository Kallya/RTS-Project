using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon : IEquipment
{
    int Damage { get; }
    float Range { get; }
    void Attack();
}

public interface ILimitedUseWeapon : IWeapon
{
    event System.Action<GameObject> OnLimitReached;
}