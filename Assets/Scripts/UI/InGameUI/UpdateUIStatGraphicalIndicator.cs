using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateUIStatGraphicalIndicator : UpdateStatGraphicalIndicator
{
    public override void SubscribeToStatChange()
    {
        PlayerInfoUIManager.Instance.OnAnyStatChanged += AnyStatChanged;
    }
}
