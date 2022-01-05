using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    // Assigned in inspector
    public List<GameObject> WeaponPrefabs = new List<GameObject>();
    public Dictionary<string, GameObject> WeaponReferences = new Dictionary<string, GameObject>();
    public static Weapons Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        foreach (GameObject weapon in WeaponPrefabs)
        {
            WeaponReferences.Add(weapon.name, weapon);
        }
    }
}
