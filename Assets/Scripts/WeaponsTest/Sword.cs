using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    public Sprite EquipSprite { get => _equipSprite; }
    public int Damage { get; } = 30;
    public float Range { get; } = 1f;

    [SerializeField] private Sprite _equipSprite;
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
