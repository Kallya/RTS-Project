using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHealthBar : UpdateStatGraphicalIndicator
{
    public override void SubscribeToStatChange()
    {
        transform.parent.GetComponent<CharacterStats>().Health.OnStatChanged += AnyStatChanged;
    }
}
