using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandsDisplayHUDSetup : MonoBehaviour
{
    public static CommandsDisplayHUDSetup Instance { get; private set; }
    
    [SerializeField] private RectTransform commandHUDPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void Setup()
    {
        List<GameObject> localCharacters = POVManager.Instance.ActiveCharacters;

        for (int i = 0; i < localCharacters.Count; i++)
        {
            RectTransform hud = Instantiate(commandHUDPrefab, transform);
            CommandHUD cmdHUD = hud.GetComponent<CommandHUD>();
            
            cmdHUD.CharacterIndex = i;
            cmdHUD.CharacterCmdProcessor = localCharacters[i].transform.GetComponent<CommandProcessor>();
        }
    }
}
