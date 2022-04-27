using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateUIStatGraphicalIndicator : UpdateStatGraphicalIndicator
{
    private void Start()
    {
        PlayerInfoUIManager.Instance.OnAnyStatChanged += AnyStatChanged;
    }
}
