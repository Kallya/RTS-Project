using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    public Sprite EquipSprite { get => _equipSprite; }
    public int Damage { get; } = 50;
    public int EnergyCost { get; } = 0;
    public float Range { get; } = 1f;

    [SerializeField] private Sprite _equipSprite;
    private Rigidbody _rb;
    private float _lastAttackTime = -s_attackRate;
    private static float s_attackRate = 1f;
    private static float s_collisionTimeAllowance = 2f; // time rb will detect collisions (essentially animation length)

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.detectCollisions = false;
    }

    private void Start()
    {
        // prevent collisions between character and its sword
        Physics.IgnoreCollision(transform.root.GetComponent<Collider>(), GetComponent<Collider>());
    }

    public bool Attack()
    {
        if (Time.time >= _lastAttackTime + s_attackRate)
        {
            StartCoroutine(AttackTime());
            _lastAttackTime = Time.time;
            return true;
        }
        else
            return false;
    }

    private IEnumerator AttackTime()
    {
        _rb.detectCollisions = true;
        yield return new WaitForSeconds(s_collisionTimeAllowance);
        _rb.detectCollisions = false;
    }
}
