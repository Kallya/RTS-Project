using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Score
{
    public event System.Action<Score> OnScoreChanged;
    public string PlayerName { get; private set; }
    public int ScoreCount
    {
        get => _scoreCount;
        set
        {
            _scoreCount = value;
            OnScoreChanged?.Invoke(this);
        }
    }
    public int CharactersRemaining
    {
        get => _charactersRemaining;
        set
        {
            _charactersRemaining = value;
            OnScoreChanged?.Invoke(this);
        }
    }

    private int _scoreCount;
    private int _charactersRemaining;

    public Score(string playerName, int scoreCount, int charactersRemaining)
    {
        PlayerName = playerName;
        ScoreCount = scoreCount;
        CharactersRemaining = charactersRemaining;
    }
}

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private List<Score> playerScores = new List<Score>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
    }
    
    
}
