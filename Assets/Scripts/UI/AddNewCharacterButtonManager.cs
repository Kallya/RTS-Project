using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddNewCharacterButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject _panel;

    public void EnableCharacterInterface()
    {
        _panel.SetActive(true);
        gameObject.SetActive(false);
        
        // increment num of characters to spawn
        ((MyNetworkManager)MyNetworkManager.singleton).CharacterNum += 1;
    }
}
