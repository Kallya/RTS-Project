using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateStatGraphicalIndicator : MonoBehaviour
{
    private Slider _indicator;
    [SerializeField] private string _thisStatName;

    private void Awake()
    {
        _indicator = GetComponent<Slider>();
    }

    public void AnyStatChanged(Stat stat)
    {
        if (stat.Name == _thisStatName)
            _indicator.value = (float)stat.Value / stat.BaseValue;
    }
}
