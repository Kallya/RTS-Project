using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// functionality to set and store customisable player settings
public class PlayerSettings : MonoBehaviour
{
    [System.Serializable]
    public class HotkeySetting
    {
        public string Name;
        public Button Button;
        public TMP_Text ButtonText;
        public string Key;
        public string DefaultKey;
    }

    [System.Serializable]
    public class AudioSetting
    {
        public string Name;
        public Slider Slider;
        public float Value;
        public float DefaultValue;
    }

    [SerializeField] private List<HotkeySetting> _hotkeySettings;
    [SerializeField] private List<AudioSetting> _audioSettings;
    private Dictionary<object, object> _uiSettingMapping = new Dictionary<object, object>(); // setting ui element, setting object
    private List<string> _usedKeys = new List<string>();
    private CanvasGroup _canvasGroup;
    private bool _waitingForInput = false;
    private string _newKey;
/*
    private static Dictionary<string, object> _defaultSettings = new Dictionary<string, object>() {
        {"Toggle Queue Commands", KeyCode.G},
        {"Toggle Scoreboard", KeyCode.Tab},
        {"Toggle Cloak", KeyCode.T},
        {"Toggle Auto Attack", KeyCode.D},
        {"Move", 1},
        {"Undo", KeyCode.S},
        {"Bail Out", KeyCode.A},
        {"Utilise", KeyCode.F},
        {"Switch to Character 1", KeyCode.Alpha1},
        {"Switch to Character 2", KeyCode.Alpha2},
        {"Switch to Character 3", KeyCode.Alpha3},
        {"Switch to Character 4", KeyCode.Alpha4},
        {"Switch to Equipment 1", KeyCode.Q},
        {"Switch to Equipment 2", KeyCode.W},
        {"Switch to Equipment 3", KeyCode.E},
        {"Switch to Equipment 4", KeyCode.R}
    };
*/

    private void Awake()
    {
        foreach (HotkeySetting setting in _hotkeySettings)
        {
            string key = PlayerPrefs.GetString(setting.Name, setting.DefaultKey);
            setting.Key = key;
            setting.ButtonText.text = key;

            _uiSettingMapping.Add(setting.Button, setting);
            _usedKeys.Add(key);

            setting.Button.onClick.AddListener(() => StartCoroutine(GetNewKey(setting.Button)));
        }

        foreach (AudioSetting setting in _audioSettings)
        {
            setting.Value = PlayerPrefs.GetFloat(setting.Name, setting.DefaultValue);

            _uiSettingMapping.Add(setting.Slider, setting);

            setting.Slider.onValueChanged.AddListener(delegate { AudioSliderChanged(setting.Slider); });
        }

        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnDisable()
    {
        foreach (HotkeySetting setting in _hotkeySettings)
        {
            PlayerPrefs.SetString(setting.Name, setting.Key);

            // setting.Button.onClick.RemoveAllListeners();
        }

        foreach (AudioSetting setting in _audioSettings)
        {
            PlayerPrefs.SetFloat(setting.Name, setting.Value);

            // setting.Slider.onValueChanged.RemoveAllListeners();
        }
    }

    private void OnGUI()
    {
        if (_waitingForInput == false)
            return;

        // read input when setting new keybinding
        if (Input.anyKeyDown)
        {
            Event e = Event.current;
            string key = e.keyCode.ToString();

            if (_usedKeys.Contains(key) == false) // check for conflicts with other keybindings
                _newKey = key;
            else
                Debug.Log("That hotkey is already used");
        }
    }

    private IEnumerator GetNewKey(Button btn)
    {
        _canvasGroup.interactable = false; // disable other ui interactions while player sets keybinding
        SetBtnColour(btn, Color.green); // distinguish keybinding being changed
        _waitingForInput = true;

        yield return new WaitUntil(() => _newKey != null);
        Debug.Log(_newKey);

        _waitingForInput = false;
        SetBtnColour(btn, Color.white);
        _canvasGroup.interactable = true;
        
        HotkeySetting setting = (HotkeySetting)_uiSettingMapping[btn];
        setting.Key = _newKey;
        setting.ButtonText.text = _newKey;

        _newKey = null; // set null again for next keybinding change
    }

    private void AudioSliderChanged(Slider slider)
    {
        AudioSetting setting = (AudioSetting)_uiSettingMapping[slider];
        setting.Value = slider.value;
    }

    private void SetBtnColour(Button btn, Color colour)
    {
        btn.GetComponent<Image>().color = colour;
    }
}