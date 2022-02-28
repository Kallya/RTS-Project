using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHealthBar : MonoBehaviour
{
    [SerializeField] private Transform _healthBarSprite;

    private Quaternion _initRot;

    private void Awake()
    {
        _initRot = transform.rotation;
    }
    
    private void Start()
    {
        transform.parent.GetComponent<CharacterStats>().Health.OnStatChanged += HealthChanged;
    }

    // stop hp bar from rotating
    private void LateUpdate()
    {
        transform.rotation = _initRot;
    }

    private void HealthChanged(Stat stat)
    {
        float healthPortion = (float)stat.Value/stat.BaseValue;
        _healthBarSprite.localScale = new Vector3(1 * healthPortion, 1f, 1f);
        _healthBarSprite.localPosition = new Vector3((healthPortion - 1) / 2, 0f, 0f); // adjust position so hp bar reduces to left
    }
}
