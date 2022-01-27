using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour, IWeapon
{   
    // weapon sprite useless on bullet 
    public Sprite EquipSprite { get => _equipSprite; }
    // is it possible to dynamically reference from gun?
    public int Damage { get; } = 10;

    private Sprite _equipSprite;
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
        CmdDestroy(gameObject);
    }

    [Command]
    private void CmdDestroy(GameObject go)
    {
        NetworkServer.Destroy(go);
    }

    // Bullet does not have an attack
    public void Attack() {}
}
