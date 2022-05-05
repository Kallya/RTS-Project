using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    // Assigned in inspector
    public List<GameObject> equipmentPrefabs = new List<GameObject>();
    public Dictionary<string, GameObject> EquipmentReferences = new Dictionary<string, GameObject>();
    public static Equipment Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        foreach (GameObject equippable in equipmentPrefabs)
            EquipmentReferences.Add(equippable.name, equippable);
    }
}
