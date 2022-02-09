using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour, IWeapon
{   
    // weapon sprite useless on bullet 
    public Sprite EquipSprite { get => _equipSprite; }
    // is it possible to dynamically reference from gun?
    public int Damage { get => _damage; }
    public float Range { get => _range; }

    private Sprite _equipSprite;
    [SerializeField] private int _damage = 10; 
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _range = 10f;
    private Vector3 s_initPos;

    private void Awake()
    {
        s_initPos = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, s_initPos) >= _range)
            Destroy(gameObject); // should I pool the bullets instead?

        transform.Translate(transform.forward * _speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (netIdentity.isServer)
            Destroy(gameObject);
    }

    // Bullet does not have an attack
    public void Attack() {}
}
