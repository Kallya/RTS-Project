using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IUtility
{
    public Sprite EquipSprite { get => _equipSprite; }

    [SerializeField] private Sprite _equipSprite;

    public void Activate()
    {
        
    }
}
