using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipment
{
    int EnergyCost { get; }
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
    float Range { get; }
    bool Attack(); // true if attack went through, false if it didn't
}
