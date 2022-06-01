using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IUtility, ILimitedUseEquippable
{
    public Sprite EquipSprite { get => _equipSprite; }
    public int EnergyCost { get; } = 0;
    public Stat Health { get; } = new Stat("Health", 200);
    public event System.Action<GameObject> OnLimitReached;

    [SerializeField] private Sprite _equipSprite;
    private Transform _character;

    private void Awake()
    {
        // set character as parent so shield doesn't move around
        _character = transform.root;
        transform.parent = null;
    }

    private void Update()
    {
        if (_character == null)
            return;

        transform.position = _character.position + _character.forward * 2f + _character.up * 2f;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _character.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    public void Activate()
    {
        // functionality in shield as damageable object
    }
/*
    // separate damageable functionality cause can't place networkidentity on child gameobject
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.transform.TryGetComponent<IWeapon>(out IWeapon weapon))
            Stats.DecreaseStat(Health, weapon.Damage);

        if (Health.Value <= 0)
            Destroy(gameObject);
    }
*/
    private void OnDestroy()
    {
        OnLimitReached?.Invoke(gameObject);
    }
}
