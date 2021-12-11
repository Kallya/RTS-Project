using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IWeapon
{
    public int Damage { get; set; }

    [SerializeField]
    private static float s_speed = 20f;
    [SerializeField]
    private static float s_range = 10f;
    private Vector3 s_initPos;

    private void Awake()
    {
        s_initPos = transform.position;
    } 

    private void Update()
    {
        if (Vector3.Distance(transform.position, s_initPos) >= s_range)
            Destroy(gameObject);

        transform.Translate(transform.forward * s_speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    public void Attack() { return; }
}
