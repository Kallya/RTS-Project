using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateEnergyBar : UpdateStatGraphicalIndicator
{
    public override void SubscribeToStatChange()
    {
        transform.root.GetComponent<CharacterStats>().Energy.OnStatChanged += AnyStatChanged;
    }
}
