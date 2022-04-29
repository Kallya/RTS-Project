using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IUtility, ILimitedUseEquippable
{
    public Sprite EquipSprite { get => _equipSprite; }
    public event System.Action<GameObject> OnLimitReached;

    [SerializeField] private Sprite _equipSprite;

    public void Activate()
    {
        
    }

    private void OnDestroy()
    {
        OnLimitReached.Invoke(gameObject);
    }
}
