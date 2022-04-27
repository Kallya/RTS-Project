using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHUDSetup : MonoBehaviour
{
    public CharacterStats Character
    {
        get => _character;
        set
        {
            _character = value;
            SetupHUD();
        }
    }
    
    [SerializeField] private Image _characterPortrait;
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private GameObject _energyBar;
    private CharacterStats _character;

    private void SetupHUD()
    {
        _characterPortrait.sprite = Character.CharacterSprite;
        Character.Health.OnStatChanged += _healthBar.GetComponent<UpdateStatGraphicalIndicator>().AnyStatChanged;
        Character.Energy.OnStatChanged += _energyBar.GetComponent<UpdateStatGraphicalIndicator>().AnyStatChanged;
    }
}
