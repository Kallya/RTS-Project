using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class Clock : MonoBehaviour
{
    public static Clock Instance { get; private set; }
    public event System.Action OnFinishGame;
    public event System.Action OnCrunchTime;

    private static double _timeAllowed = 180;
    private bool _crunchTimeEnabled;
    private double _initTime;
    [SerializeField] private TMP_Text _clockText;
    [SerializeField] private Image _clockImage;

    private void Awake()
    {
        Instance = this;

        _initTime = NetworkTime.time;
    }

    private void Update()
    {
        double remainingTime = _timeAllowed - NetworkTime.time + _initTime;

        if (remainingTime <= 0)
            OnFinishGame?.Invoke();

        int remainingMin = (int)(remainingTime / 60);
        int remainingSec = (int)remainingTime % 60;

        _clockText.text = $"{remainingMin}:{MakeTwoDigit(remainingSec)}";

        if (_crunchTimeEnabled == false && remainingTime <= 60)
        {
            _clockImage.color = Color.red;
            _crunchTimeEnabled = true;
            OnCrunchTime?.Invoke();
        }
    }

    private string MakeTwoDigit(int num)
    {
        if (num < 10)
            return $"0{num}";
        else
            return num.ToString();
    }
}
