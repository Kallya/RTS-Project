using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalCanvas : MonoBehaviour
{
    private static UniversalCanvas s_instance;
    [SerializeField] private GameObject _settingsMenu;
    
    private void Awake()
    {
        // prevent duplicates due to dontdestroyonload
        if (s_instance != null)
            return;

        s_instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _settingsMenu.SetActive(!_settingsMenu.activeSelf);
    }
}
