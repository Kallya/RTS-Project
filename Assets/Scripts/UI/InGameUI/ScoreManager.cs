using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private SyncDictionary<string, int> _playerScores = new SyncDictionary<string, int>();

    private void Awake()
    {
        Instance = this;
    }

    
    
}
