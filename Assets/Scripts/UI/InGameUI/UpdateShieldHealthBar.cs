using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateShieldHealthBar : UpdateStatGraphicalIndicator
{
    [SerializeField] private Shield _shield;

    public override void SubscribeToStatChange()
    {
        _shield.Health.OnStatChanged += AnyStatChanged;
    }
}
