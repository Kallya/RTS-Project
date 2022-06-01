using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private static FPSCounter s_instance;
    private GUIStyle _style = new GUIStyle();
    private int _fps;
    private float _lastTime;

    private void Awake()
    {
        if (s_instance != null)
            return;

        s_instance = this;
        _style.fontSize = 48;
        _style.normal.textColor = Color.yellow;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Time.time >= _lastTime + 1f)
        {
            _fps = (int)(1f / Time.unscaledDeltaTime);
            _lastTime = Time.time;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 100, 25), "FPS: " + _fps.ToString(), _style);
    }
}
