using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    private DamageableObjectStats _damageableStats;

    private void Awake()
    {
        _damageableStats = GetComponent<DamageableObjectStats>();
    }

    private void Update()
    {
        if (_damageableStats.Health.Value <= 0)
            DestroyObject();
    }

    public virtual void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void TakeDamage(int damage)
    {
        Stats.DecreaseStat(_damageableStats.Health, damage);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<IWeapon>(out IWeapon weaponCollision))
        {
            TakeDamage(weaponCollision.Damage);
            Debug.Log($"{gameObject.name} took {weaponCollision.Damage} damage");
        }
    }
}
