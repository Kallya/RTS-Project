using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHealthBar : UpdateStatGraphicalIndicator
{
    public override void SubscribeToStatChange()
    {
        transform.root.GetComponent<CharacterStats>().Health.OnStatChanged += AnyStatChanged;
    }
}
