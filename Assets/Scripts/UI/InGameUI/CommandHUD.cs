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
    [SerializeField] private TMP_Text _cmdTextPrefab;
    [SerializeField] private TMP_Text _characterIdentifier;
    private CommandProcessor _characterCmdProcessor;

    private void SetupHUD()
    {
        _characterCmdProcessor.OnCommandQueued += CommandQueued;
        _characterCmdProcessor.OnCommandDequeued += CommandDequeued;

        _characterIdentifier.text = $"Character {CharacterIndex+1}";
    }

    private void CommandQueued(IQueueableCommand command)
    {
        TMP_Text cmdText = Instantiate(_cmdTextPrefab, _contentContainer);
        cmdText.text = command.Name;
    }

    private void CommandDequeued()
    {
        Destroy(_contentContainer.GetChild(0).gameObject);
    }
}
