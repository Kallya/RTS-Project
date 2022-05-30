using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ErrorMessageType
{
    NotHoldingUtilisable,
    NotHoldingQueueableEquipment,
    NoPlayerName,
    UnavailablePlayerName,
    InsufficientEnergy, 
    DeadCharacterSwitch,
    CannotBailOut,
    HotkeyConflict
}

public class ErrorNotifier : MonoBehaviour
{
    public static ErrorNotifier Instance { get; private set; }
    public static Dictionary<ErrorMessageType, string> ErrorMessages = new Dictionary<ErrorMessageType, string>() {
        {ErrorMessageType.NotHoldingUtilisable, "You are not holding anything utilisable."},
        {ErrorMessageType.NotHoldingQueueableEquipment, "You are not holding queueable equipment."},
        {ErrorMessageType.NoPlayerName, "Please enter your player name."},
        {ErrorMessageType.UnavailablePlayerName, "Your player name has already been taken by someone in the lobby."},
        {ErrorMessageType.InsufficientEnergy, "You have insufficient energy to use this equipment."},
        {ErrorMessageType.DeadCharacterSwitch, "You cannot switch to a dead character."},
        {ErrorMessageType.CannotBailOut, "You are within 50 units of an enemy. You cannot bail out."},
        {ErrorMessageType.HotkeyConflict, "This key is conflicting with another keybinding"}
    };

    [SerializeField] private List<GameObject> _errorObjectPool;
    private List<TMP_Text> _errorObjectText = new List<TMP_Text>();
    private int _poolIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (GameObject error in _errorObjectPool)
            _errorObjectText.Add(error.GetComponent<TMP_Text>());
    }

    public void GenerateErrorMsg(string msg)
    {
        StartCoroutine(GenerateErrorMsgC(msg));
    }

    private IEnumerator GenerateErrorMsgC(string msg)
    {
        int _lastMsgIndex = _poolIndex - 1;
        if (_lastMsgIndex < 0)
            _lastMsgIndex = _errorObjectPool.Count - 1;

        // prevent error msg flooding due to polling nature of input detection
        // though this may result in missed msgs in specific cases
        if (_errorObjectPool[_lastMsgIndex].activeSelf == true && _errorObjectText[_lastMsgIndex].text == msg)
            yield break;

        _errorObjectText[_poolIndex].text = msg;
        _errorObjectPool[_poolIndex].SetActive(true);

        int originalIndex = _poolIndex;

        // increment poolIndex to access next errorObj in pool
        // and reset if its reached the end of the pool
        _poolIndex += 1;
        if (_poolIndex >= _errorObjectPool.Count)
            _poolIndex = 0;

        yield return new WaitForSeconds(3f); // error message remains for 3 sec

        _errorObjectPool[originalIndex].SetActive(false);
    } 
}
