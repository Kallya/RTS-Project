using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePlayerButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject _addPlayerButton;

    public void DisableCharacterInterface()
    {
        _addPlayerButton.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }
}
