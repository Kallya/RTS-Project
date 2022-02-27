using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHealthBar : MonoBehaviour
{
    [SerializeField] private Transform _healthBarSprite;
    
    private void Start()
    {
        transform.parent.GetComponent<CharacterStats>().Health.OnStatChanged += HealthChanged;
    }

    private void HealthChanged(Stat stat)
    {
        _healthBarSprite.localScale = new Vector3(1 * (stat.Value/stat.BaseValue), 1f, 1f);
        _healthBarSprite.Translate(new Vector3((1 - stat.Value/stat.BaseValue) / 2, 0f, 0f));
    }
}
