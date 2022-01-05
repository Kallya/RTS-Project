using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour, IWeapon
{   
    // is it possible to dynamically reference from gun?
    public int Damage { get; } = 10;

    [SerializeField] private static float s_speed = 20f;
    [SerializeField] private static float s_range = 10f;
    private Vector3 s_initPos;

    private void Awake()
    {
        s_initPos = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, s_initPos) >= s_range)
            Destroy(gameObject); // should I pool the bullets instead?

        transform.Translate(transform.forward * s_speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    // Bullet does not have an attack
    public void Attack() {}
}
