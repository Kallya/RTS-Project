using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateEnergyBar : UpdateStatGraphicalIndicator
{
    public override void SubscribeToStatChange()
    {
        transform.parent.GetComponent<CharacterStats>().Energy.OnStatChanged += AnyStatChanged;
    }
}
