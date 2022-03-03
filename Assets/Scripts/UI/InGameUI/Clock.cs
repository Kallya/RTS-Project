using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class Clock : MonoBehaviour
{
    private static double _timeAllowed = 180;
    private bool _crunchTimeEnabled = false;
    private double _initTime;
    [SerializeField] private TMP_Text _clockText;
    [SerializeField] private Image _clockImage;

    private void Awake()
    {
        _initTime = NetworkTime.time;
    }

    private void Update()
    {
        double remainingTime = _timeAllowed - NetworkTime.time + _initTime;

        // if (remainingTime < 0)
            // change to end game scene

        int remainingMin = (int)(remainingTime / 60);
        int remainingSec = (int)remainingTime % 60;

        _clockText.text = $"{remainingMin}:{MakeTwoDigit(remainingSec)}";

        if (_crunchTimeEnabled == false && remainingTime <= 60)
        {
            _clockImage.color = Color.red;
            _crunchTimeEnabled = true;
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
