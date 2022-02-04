using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// could this inherit from Gun instead? (good practice?)
public class Shotgun : MonoBehaviour, IAutoAttackWeapon
{
    public Sprite EquipSprite { get => _equipSprite; }
    public int Damage { get; } = 5;
    public float Range { get; } = 5f;

    [SerializeField] private Sprite _equipSprite;
    [SerializeField] private Transform _bullet;
    // 6 shotpoints?
    [SerializeField] private List<Transform> _shotPoints;
    private static float s_fireRate = 0.6f;
    private float _lastShotTime = -s_fireRate;

    public void Attack()
    {
        if (Time.time >= _lastShotTime + s_fireRate)
        {
            InstantiateBullets();
            _lastShotTime = Time.time;
        }
    }

    private void InstantiateBullets()
    {
        foreach (Transform shotPoint in _shotPoints)
            ObjectSpawner.Instance.CmdSpawnNetworkObject(1, shotPoint.position, transform.rotation * _bullet.rotation);
    }
}