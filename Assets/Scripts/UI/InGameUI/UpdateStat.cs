using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateStat : MonoBehaviour
{
    private TextMeshProUGUI _statText;
    [SerializeField] private string _thisStatName;

    private void Awake()
    {
        _statText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        PlayerInfoUIManager.Instance.OnAnyStatChanged += AnyStatChanged;
    }

    private void AnyStatChanged(Stat stat)
    {
        if (stat.Name == _thisStatName)
        {
            if (gameObject.tag == "FractionalStat")
                _statText.text = $"{stat.Value}/{stat.BaseValue}";
            else if (gameObject.tag == "IntegerStat")
                _statText.text = stat.Value.ToString();
        }
    }
}
