using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AssaultRifle : MonoBehaviour, IWeapon
{
    public Sprite EquipSprite { get => _equipSprite; }
    public int EnergyCost { get; } = 1;
    public int Damage { get; } = 10;
    public float Range { get; } = 10f;

    [SerializeField] private Sprite _equipSprite;
    [SerializeField] private Transform _bullet;
    [SerializeField] private Transform _shotPoint;
    private static float s_fireRate = 0.3f;
    // initialised to negative of fireRate so player can instantly start shooting on spawn
    private float _lastShotTime = -s_fireRate;

    public bool Attack()
    {
        if (Time.time >= _lastShotTime + s_fireRate)
        {
            InstantiateBullet();
            _lastShotTime = Time.time;
            return true;
        }
        else
            return false;
    }


    private void InstantiateBullet()
    {
        ObjectSpawner.Instance.CmdSpawnNetworkObject(
            _bullet.name, 
            _shotPoint.position, 
            transform.rotation * _bullet.rotation, 
            NetworkClient.connection as NetworkConnectionToClient
        );
    }
}
