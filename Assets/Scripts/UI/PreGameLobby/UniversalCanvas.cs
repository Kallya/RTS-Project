using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalCanvas : MonoBehaviour
{
    [SerializeField] private GameObject _settingsMenu;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _settingsMenu.SetActive(!_settingsMenu.activeSelf);
    }
}
