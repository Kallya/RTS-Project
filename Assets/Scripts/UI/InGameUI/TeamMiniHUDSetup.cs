using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMiniHUDSetup : MonoBehaviour
{
    public static TeamMiniHUDSetup Instance { get; private set; }
    
    [SerializeField] private RectTransform characterHUDPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void Setup()
    {
        List<GameObject> localCharacters = POVManager.Instance.ActiveCharacters;

        for (int i = 0; i < localCharacters.Count; i++)
        {
            RectTransform hud = Instantiate(characterHUDPrefab, transform);
            hud.anchoredPosition = new Vector2(-75 + i*50, 0f);
            hud.GetComponent<CharacterHUDSetup>().Character = localCharacters[i].transform.GetComponent<CharacterStats>();
        }
    }
}
