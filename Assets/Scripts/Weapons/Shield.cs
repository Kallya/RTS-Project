using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IUtility, ILimitedUseEquippable
{
    public Sprite EquipSprite { get => _equipSprite; }
    public int EnergyCost { get; } = 0;
    public event System.Action<GameObject> OnLimitReached;

    private CharacterStats _characterStats;
    private int _health = 100;

    [SerializeField] private Sprite _equipSprite;

    private void Awake()
    {
        _characterStats = GetComponent<CharacterStats>();
    }

    public void Activate()
    {
        // functionality in shield as damageable object
    }

    // separate functionality cause can't place networkidentity on child gameobject
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.transform.TryGetComponent<IWeapon>(out IWeapon weapon))
            _health -= weapon.Damage - _characterStats.Defence.Value;
    }

    private void OnDestroy()
    {
        OnLimitReached?.Invoke(gameObject);
    }
}
