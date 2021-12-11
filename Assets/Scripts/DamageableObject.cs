using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    public int health { get; set; }

    private void Update()
    {
        if (health <= 0)
            DestroyObject();
    }

    public virtual void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<IWeapon>(out IWeapon weaponCollision))
        {
            TakeDamage(weaponCollision.Damage);
            Debug.Log($"Took {weaponCollision.Damage} damage");
        }
    }
}
