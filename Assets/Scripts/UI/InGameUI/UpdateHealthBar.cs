using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHealthBar : UpdateStatBar
{
    public override void SubscribeToStatChange()
    {
        transform.parent.GetComponent<CharacterStats>().Health.OnStatChanged += StatChanged;
    }
}
