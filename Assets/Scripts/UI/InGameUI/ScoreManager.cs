using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

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

    public Score(string playerName, int charactersRemaining)
    {
        PlayerName = playerName;
        CharactersRemaining = charactersRemaining;
        ScoreCount = 0;
    }
}

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private List<Score> _playerScores = new List<Score>();
    private Dictionary<string, ScoreTextReferences> _scoreTextRefs = new Dictionary<string, ScoreTextReferences>();
    [SerializeField] private RectTransform _teamInfoRowPrefab;
    [SerializeField] private Transform _infoBoard;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitialiseScores(); // reinitialise to update UI
    }

    public void SetScoreboard(string[] playerNames, int[] teamSizes)
    {
        for (int i = 0; i < playerNames.Length; i++)
        {
            // set position on scoreboard
            RectTransform scoreRow = Instantiate(_teamInfoRowPrefab, _infoBoard);
            scoreRow.offsetMin = new Vector2(0f, 66 - i*50);
            scoreRow.offsetMax = new Vector2(0f, 66 - (i+1)*50);

            // setup for ui to update when score updates
            ScoreTextReferences textComponents = scoreRow.GetComponent<ScoreTextReferences>();
            string playerName = playerNames[i];
            int teamSize = teamSizes[i];

            _scoreTextRefs.Add(playerName, textComponents);

            Score playerScore = new Score(playerName, teamSize);
            _playerScores.Add(playerScore);
            playerScore.OnScoreChanged += ScoreChanged;

            textComponents.TeamNameText.text = playerName;
        }
    }

    private void ScoreChanged(Score score)
    {
        ScoreTextReferences textRef = _scoreTextRefs[score.PlayerName];
        
        textRef.PointsText.text = score.ScoreCount.ToString();
        textRef.CharactersRemainingText.text = score.CharactersRemaining.ToString();
    }

    private void InitialiseScores()
    {
        foreach (Score score in _playerScores)
        {
            score.CharactersRemaining = score.CharactersRemaining;
            score.ScoreCount = score.ScoreCount;
        }
    }
}
