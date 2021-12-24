using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour, IWeapon
{
    public int Damage { get; set; }

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
            NetworkServer.Destroy(gameObject);

        transform.Translate(transform.forward * s_speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        NetworkServer.Destroy(gameObject);
    }

    public void Attack() {}
}
