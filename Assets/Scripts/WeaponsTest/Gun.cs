using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Gun : MonoBehaviour, IWeapon
{
    public int Damage { get; } = 10;

    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _shotPoint;
    private static float s_fireRate = 0.3f;
    // initialised to negative of fireRate so player can instantly start shooting on spawn
    private float _lastShotTime = -s_fireRate;

    public void Attack()
    {
        if (Time.time >= _lastShotTime + s_fireRate)
        {
            InstantiateBullet();
            _lastShotTime = Time.time;
        }
    }

    private void InstantiateBullet()
    {
        ObjectSpawner.Instance.CmdSpawnNetworkObject(0, _shotPoint.position, transform.rotation * _bullet.transform.rotation);
    }
}
