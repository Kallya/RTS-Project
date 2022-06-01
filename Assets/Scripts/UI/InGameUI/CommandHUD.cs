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
    private Dictionary<IQueueableCommand, GameObject> _commandTextDict = new Dictionary<IQueueableCommand, GameObject>();

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

        // cache command text for deletion
        _commandTextDict.Add(command, cmdText.gameObject);
    }

    private void CommandCompleted(IQueueableCommand command)
    {
        // Delete command based on dict
        // cause destroying oldest command may result in two destroy calls to same command
        // due to speed of completion event call
        // leaving command text undeleted even when command is finished
        Destroy(_commandTextDict[command]);
        _commandTextDict.Remove(command);
    }

    // Delete command at end of queue
    private void CommandUndone()
    {
        Destroy(_contentContainer.GetChild(_contentContainer.childCount-1).gameObject);
    }
}
