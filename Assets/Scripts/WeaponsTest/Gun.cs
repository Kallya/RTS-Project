using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Gun : MonoBehaviour, IWeapon
{
    public int Damage { get; } = 10;

    [SerializeField]
    private GameObject _bullet;
    [SerializeField]
    private Transform _shotPoint;
    private static float s_fireRate = 0.3f;
    private bool _canShoot = true;

    public void Attack()
    {
        if (_canShoot == true)
        {
            RpcInstantiateBullet();
            StartCoroutine(StallFire());
        }
    }

    [ClientRpc]
    private void RpcInstantiateBullet()
    {
        GameObject bulletInstance = Instantiate(_bullet, _shotPoint.position, transform.rotation * _bullet.transform.rotation);
        bulletInstance.GetComponent<Bullet>().Damage = Damage;
        NetworkServer.Spawn(bulletInstance);
    }

    private IEnumerator StallFire()
    {
        _canShoot = false;
        yield return new WaitForSeconds(s_fireRate);
        _canShoot = true;
    }
}
