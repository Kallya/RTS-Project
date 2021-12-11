using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, IWeapon
{
    public int Damage { get; } = 10;

    [SerializeField]
    private Transform _bullet;
    [SerializeField]
    private Transform _shotPoint;
    private static float s_fireRate = 0.3f;
    private bool _canShoot = true;

    public void Attack()
    {
        if (_canShoot == true)
        {
            Instantiate(_bullet, _shotPoint.position, transform.rotation * _bullet.rotation)
                .GetComponent<Bullet>().Damage = Damage;
            StartCoroutine(StallFire());
        }
    }

    private IEnumerator StallFire()
    {
        _canShoot = false;
        yield return new WaitForSeconds(s_fireRate);
        _canShoot = true;
    }
}
