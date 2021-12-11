using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private static float speed;

    private Transform player;
    private PlayerInput playerInput;

    private void Awake()
    {
        player = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();

        speed = 25f;
    }

    private void Update()
    {
        player.Translate(new Vector3(playerInput.HorizontalAxis * speed * Time.deltaTime, 
                                    0f,
                                    playerInput.VerticalAxis * speed * Time.deltaTime));
    }
}
