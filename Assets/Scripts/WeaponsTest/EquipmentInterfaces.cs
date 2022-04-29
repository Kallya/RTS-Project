using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipment
{
    Sprite EquipSprite { get; }
}

public interface ILimitedUseEquippable : IEquipment
{
    event System.Action<GameObject> OnLimitReached;
}

public interface IUtility : IEquipment
{
    void Activate();
}

public interface IWeapon : IEquipment
{
    int Damage { get; }
    int EnergyCost { get; }
    float Range { get; }
    void Attack();
}
