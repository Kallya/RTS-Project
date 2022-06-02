using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class EndgameManager : MonoBehaviour
{
    public static EndgameManager Instance { get; private set; }

    [SerializeField] private TMP_Text _outcomeText;
    [SerializeField] private TMP_Text _winnersText;
    [SerializeField] private RectTransform _teamInfoRowPrefab;
    [SerializeField] private Transform _infoBoard;
    
    private void Awake()
    {
        Instance = this;
    }

    public void Setup(Score[] finalScores, int[] winnerIds)
    {
        MyNetworkManager netManager = MyNetworkManager.singleton as MyNetworkManager;

        // set outcome heading
        if (winnerIds.Length > 1)
        {
            _outcomeText.text = "Draw";
            _outcomeText.color = Color.gray;
        }
        else if (winnerIds[0] == netManager.ConnId)
        {
            _outcomeText.text = "Victory";
            _outcomeText.color = Color.cyan;
        }
        else
        {
            _outcomeText.text = "Defeat";
            _outcomeText.color = Color.red;
        }

        // set subheading revealing (possibly joint) winners
        string winnerText = "Winners: " + finalScores[0].PlayerName;
        for (int i = 1; i < winnerIds.Length; i++)
            winnerText += ", " + finalScores[i].PlayerName;
        _winnersText.text = winnerText;
        
        // generate ending scoreboard
        foreach (Score score in finalScores)
        {
            RectTransform scoreRow = Instantiate(_teamInfoRowPrefab, _infoBoard);
            ScoreTextReferences textComponents = scoreRow.GetComponent<ScoreTextReferences>();

            textComponents.TeamNameText.text = score.PlayerName;
            textComponents.PointsText.text = score.ScoreCount.ToString();
            textComponents.CharactersRemainingText.text = score.CharactersRemaining.ToString();
        }

        gameObject.SetActive(true);          
    }

    public void ReturnHomeBtnClick()
    {
        if (NetworkClient.localPlayer.isServer)
            MyNetworkManager.singleton.StopHost();
        else
            MyNetworkManager.singleton.StopClient();
    }
}
