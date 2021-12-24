using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum Player : byte
{
    player1,
    player2
}

public class Initialise : NetworkBehaviour
{
    [SyncVar(hook=nameof(DisplayHealth))] private int _health;

    private GameObject player1;
    private GameObject player2;

    [SyncVar(hook=nameof(ChangePlayer))] private Player _currPlayer;

    private void DisplayHealth(int oldHealth, int newHealth)
    {
        Debug.Log($"{netIdentity.connectionToClient.connectionId}'s health is now {newHealth}");
    }

    private void ChangePlayer(Player oldPlayer, Player newPlayer)
    {
        switch(newPlayer)
        {
            case Player.player1:
                player1.SetActive(true);
                player2.SetActive(false);
                break;
            case Player.player2:
                player2.SetActive(true);
                player1.SetActive(false);
                break;
        }
    }

    public override void OnStartServer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        player1 = players[1];
        player2 = players[0];

        _health = 100;
        _currPlayer = Player.player1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _health -= 10;
        }

        if (Input.GetKeyDown(KeyCode.A))
            ChangePlayer(Player.player1);
        if (Input.GetKeyDown(KeyCode.F))
            ChangePlayer(Player.player2);
    }

    [Command]
    private void ChangePlayer(Player player)
    {
        _currPlayer = player;
    }
}
