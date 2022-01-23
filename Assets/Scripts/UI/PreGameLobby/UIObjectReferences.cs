using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// use to get reference to disabled UI elements
public class UIObjectReferences : MonoBehaviour
{
    public static UIObjectReferences Instance { get; private set; }
    public GameObject CharacterSetupUI;
    public GameObject EventSystem;

    private void Awake()
    {
        Instance = this;
    }
}
