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

    public Dictionary<string, KeyCode> s_HotkeyMappings = new Dictionary<string, KeyCode>();
    public static PlayerSettings Instance { get; private set; }
    public event System.Action<string> OnAudioLevelChanged;

    [SerializeField] private List<HotkeySetting> _hotkeySettings; // set in inspector
    [SerializeField] private List<AudioSetting> _audioSettings; // set in inspector
    private KeyCode[] _allKeyCodes;
    private Dictionary<object, object> _uiSettingMapping = new Dictionary<object, object>(); // setting ui element, setting object
    private List<string> _usedKeys = new List<string>();
    private CanvasGroup _canvasGroup;
    private bool _waitingForInput = false;
    private string _newKey;
/*
    Default Mapping
    private static Dictionary<string, object> _defaultSettings = new Dictionary<string, object>() {
        {"Toggle Queue Commands", KeyCode.Space},
        {"Toggle Scoreboard", KeyCode.Tab},
        {"Toggle Cloak", KeyCode.T},
        {"Toggle Auto Attack", KeyCode.D},
        {"Move", KeyCode.Mouse1},
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
        gameObject.SetActive(false);

        Instance = this;
        _canvasGroup = GetComponent<CanvasGroup>();
        _allKeyCodes = (KeyCode[]) System.Enum.GetValues(typeof(KeyCode));

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
            setting.Slider.value = setting.Value;

            _uiSettingMapping.Add(setting.Slider, setting);

            setting.Slider.onValueChanged.AddListener(delegate { AudioSliderChanged(setting.Slider); });
        }
    }

    // update playerprefs when menu closed
    private void OnDisable()
    {
        foreach (HotkeySetting setting in _hotkeySettings)
        {
            PlayerPrefs.SetString(setting.Name, setting.Key);

            // cache in game reference since PlayerPrefs can't store KeyCode
            if (s_HotkeyMappings.Count == 0) // only on first update when empty
                s_HotkeyMappings.Add(setting.Name, GetKeyCode(setting.Key));
            else
                s_HotkeyMappings[setting.Name] = GetKeyCode(setting.Key);
        }

        foreach (AudioSetting setting in _audioSettings)
        {
            PlayerPrefs.SetFloat(setting.Name, setting.Value);
        }
    }

    // apparently polling is more efficient than OnGUI
    private void Update()
    {
        if (_waitingForInput == false)
            return;

        for (int i = 1; i < _allKeyCodes.Length; i++)
        {
            string key = _allKeyCodes[i].ToString();

            if (Input.GetKeyDown(_allKeyCodes[i]))
            {
                if (_usedKeys.Contains(key) == false)
                    _newKey = key;
                else
                    Debug.Log("This key is already used");
            }
        }
    }

    private IEnumerator GetNewKey(Button btn)
    {
        _canvasGroup.interactable = false; // disable other ui interactions while player sets keybinding
        SetBtnColour(btn, Color.green); // distinguish keybinding being changed
        _waitingForInput = true;

        yield return new WaitUntil(() => _newKey != null);

        _waitingForInput = false;
        SetBtnColour(btn, Color.white);
        _canvasGroup.interactable = true;
        
        HotkeySetting setting = (HotkeySetting)_uiSettingMapping[btn];

        // update used keys
        _usedKeys.Remove(setting.Key);
        _usedKeys.Add(_newKey);

        setting.Key = _newKey;
        setting.ButtonText.text = _newKey;

        _newKey = null; // set null again for next keybinding change
    }

    private void AudioSliderChanged(Slider slider)
    {
        AudioSetting setting = (AudioSetting)_uiSettingMapping[slider];
        setting.Value = slider.value;

        OnAudioLevelChanged?.Invoke(setting.Name);
    }

    private void SetBtnColour(Button btn, Color colour)
    {
        btn.GetComponent<Image>().color = colour;
    }

    // convert string to keycode
    private KeyCode GetKeyCode(string key)
    {
        return (KeyCode) System.Enum.Parse(typeof(KeyCode), key);
    }
}
