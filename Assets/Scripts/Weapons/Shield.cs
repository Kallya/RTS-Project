using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IUtility, ILimitedUseEquippable
{
    public Sprite EquipSprite { get => _equipSprite; }
    public int EnergyCost { get; } = 0;
    public event System.Action<GameObject> OnLimitReached;

    private int _health = 200;

    [SerializeField] private Sprite _equipSprite;

    private void Awake()
    {
        // set character as parent so shield doesn't move around
        transform.parent = transform.root;
    }

    public void Activate()
    {
        // functionality in shield as damageable object
    }

    // separate damageable functionality cause can't place networkidentity on child gameobject
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.transform.TryGetComponent<IWeapon>(out IWeapon weapon))
            _health -= weapon.Damage;

        if (_health <= 0)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OnLimitReached?.Invoke(gameObject);
    }
}
