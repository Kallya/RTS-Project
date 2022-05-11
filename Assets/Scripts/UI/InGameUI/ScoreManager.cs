using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public event System.Action<Score[]> OnGameFinish;

    private Dictionary<int, Score> _playerScores = new Dictionary<int, Score>();
    private Dictionary<string, ScoreTextReferences> _scoreTextRefs = new Dictionary<string, ScoreTextReferences>();
    private int _inactivePlayers = 0;
    [SerializeField] private RectTransform _teamInfoRowPrefab;
    [SerializeField] private Transform _infoBoard;

    private void Awake()
    {
        Instance = this;
    }

    public void SetScoreboard(int[] connectionIds, string[] playerNames, int[] teamSizes)
    {
        for (int i = 0; i < playerNames.Length; i++)
        {
            // set position on scoreboard
            RectTransform scoreRow = Instantiate(_teamInfoRowPrefab, _infoBoard);
            scoreRow.offsetMin = new Vector2(0f, 66 - i*50);
            scoreRow.offsetMax = new Vector2(0f, 66 - (i+1)*50);

            // setup for ui to update when score updates
            ScoreTextReferences textComponents = scoreRow.GetComponent<ScoreTextReferences>();
            int connId = connectionIds[i];
            string playerName = playerNames[i];
            int teamSize = teamSizes[i];

            _scoreTextRefs.Add(playerName, textComponents);

            Score playerScore = new Score(playerName, teamSize);
            _playerScores.Add(connId, playerScore);
            playerScore.OnScoreChanged += ScoreChanged;

            textComponents.TeamNameText.text = playerName;
        }

        InitialiseScores(); // reinitialise to update UI
        Clock.Instance.OnFinishGame += FinishGame;
    }

    private void ScoreChanged(Score score)
    {
        ScoreTextReferences textRef = _scoreTextRefs[score.PlayerName];
        
        textRef.PointsText.text = score.ScoreCount.ToString();
        textRef.CharactersRemainingText.text = score.CharactersRemaining.ToString();

        if (!isServer)
            return; 

        if (score.CharactersRemaining == 0)
            _inactivePlayers += 1;
        
        // finish game if only one player left
        if (_inactivePlayers == _playerScores.Count - 1)
            FinishGame();
    }

    private void InitialiseScores()
    {
        foreach (Score score in _playerScores.Values)
        {
            score.CharactersRemaining = score.CharactersRemaining;
            score.ScoreCount = score.ScoreCount;
        }
    }

    public void UpdateScore(int killerConnId, int victimConnId)
    {
        _playerScores[victimConnId].CharactersRemaining -= 1;

        // inequality not registering as intended on client
        // team kill doesn't increase score
        if (killerConnId != victimConnId)
            _playerScores[killerConnId].ScoreCount += 1;
    }

    [Server]
    private void FinishGame()
    {
        Clock.Instance.OnFinishGame -= FinishGame;

        Score[] scores = _playerScores.Values.ToArray();
        List<Score> winners = new List<Score>();

        // go through comparing scores and adding to list of winners
        // cause there could be multiple players with same score
        for (int i = 1; i < scores.Length; i++)
        {
            if (scores[i].ScoreCount > winners[0].ScoreCount)
            {
                winners.Clear();
                winners.Add(scores[i]);
            }
            else if (scores[i].ScoreCount == winners[0].ScoreCount)
                winners.Add(scores[i]);
        }

        OnGameFinish?.Invoke(winners.ToArray());
    }
}

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

    public Score(string playerName, int charactersRemaining, int scoreCount=0)
    {
        PlayerName = playerName;
        CharactersRemaining = charactersRemaining;
        ScoreCount = scoreCount;
    }
}

public static class ScoreSerialiser
{
    public static void WriteScore(this NetworkWriter writer, Score value)
    {
        writer.WriteString(value.PlayerName);
        writer.WriteInt(value.CharactersRemaining);
        writer.WriteInt(value.ScoreCount);
    }

    public static Score ReadScore(this NetworkReader reader)
    {
        return new Score(reader.ReadString(), reader.ReadInt(), reader.ReadInt());
    }
}
