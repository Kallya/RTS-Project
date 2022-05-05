using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateStatGraphicalIndicator : MonoBehaviour
{
    [SerializeField] private Slider _indicator;
    [SerializeField] private string _thisStatName;

    private void Start()
    {
        SubscribeToStatChange();
    }

    public virtual void SubscribeToStatChange() { }

    public void AnyStatChanged(Stat stat)
    {
        // useless condition for specific stat updates
        if (stat.Name == _thisStatName)
            _indicator.value = (float)stat.Value / stat.BaseValue;
    }
}
