using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// use to get reference to disabled UI elements
public class UIObjectReferences : MonoBehaviour
{
    public static UIObjectReferences Instance { get; private set; }
    public GameObject CharacterSetupUI;
    public TMP_Text MapName;

    private void Awake()
    {
        Instance = this;
    }
}
