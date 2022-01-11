using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IAutoAttackWeapon
{
    public int Damage { get; } = 30;
    public float Range { get; } = 1f;

    private Rigidbody _rb;
    private bool _canAttack = true;
    private static float s_attackRate = 1f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.detectCollisions = false;
    }

    // fix fix fix

    public void Attack()
    {
        if (_canAttack == true)
        {
        }
    }
}
