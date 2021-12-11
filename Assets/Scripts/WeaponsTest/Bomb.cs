using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, ILimitedUseWeapon
{
    public int Damage { get; } = 80;
    public GameObject player { get; private set; }
    public event System.Action<GameObject> OnLimitReached;

    public void Attack()
    {
        SetTrigger();
    }

    private void SetTrigger()
    {
        OnLimitReached?.Invoke(gameObject);
        transform.parent = null;
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // might need to change this method of distinguishing
        if (other.tag == "Enemy")
        {
            Debug.Log("Explosion");
            Destroy(gameObject);
        }
    }
}
