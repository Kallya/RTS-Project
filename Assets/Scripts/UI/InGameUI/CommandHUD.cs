using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommandHUD : MonoBehaviour
{
    public CommandProcessor CharacterCmdProcessor
    {
        get => _characterCmdProcessor;
        set
        {
            _characterCmdProcessor = value;
            SetupHUD();
        }
    }
    public int CharacterIndex;
    
    [SerializeField] private Transform _contentContainer;
    [SerializeField] private GameObject _cmdTextPrefab;
    [SerializeField] private TMP_Text _characterIdentifier;
    private CommandProcessor _characterCmdProcessor;

    private void SetupHUD()
    {
        _characterCmdProcessor.OnCommandQueued += CommandQueued;
        _characterCmdProcessor.OnCommandCompleted += CommandCompleted;
        _characterCmdProcessor.OnCommandUndone += CommandUndone;

        _characterIdentifier.text = $"Character {CharacterIndex+1}";
    }

    private void CommandQueued(IQueueableCommand command)
    {
        TMP_Text cmdText = Instantiate(_cmdTextPrefab, _contentContainer).GetComponent<TMP_Text>();
        cmdText.text = command.Name;
    }

    // Delete command at start of queue
    private void CommandCompleted()
    {
        TMP_Text text = _contentContainer.GetChild(0).GetComponent<TMP_Text>();
        Debug.Log(text.text);

        Destroy(_contentContainer.GetChild(0).gameObject);
    }

    // Delete command at end of queue
    private void CommandUndone()
    {
        Destroy(_contentContainer.GetChild(_contentContainer.childCount-1).gameObject);
    }
}
