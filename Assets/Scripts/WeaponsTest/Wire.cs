using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour, IUtility, ILimitedUseEquippable
{
    public Sprite EquipSprite { get => _equipSprite; }
    public event System.Action<GameObject> OnLimitReached;

    private Sprite _equipSprite;
    
    public void Activate()
    {
        OnLimitReached.Invoke(gameObject);
    }
}
