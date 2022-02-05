using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipment
{
    Sprite EquipSprite { get; }
}

public interface IUtility : IEquipment
{
    void Activate();
}
