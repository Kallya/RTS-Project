using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPortraits : MonoBehaviour
{
    // Assigned in inspector
    public List<Sprite> PortraitSprites = new List<Sprite>();
    public Dictionary<string, Sprite> PortraitReferences = new Dictionary<string, Sprite>();
    public static CharacterPortraits Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        foreach (Sprite portrait in PortraitSprites)
        {
            PortraitReferences.Add(portrait.name, portrait);
        }
    }
}
