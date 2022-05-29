using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    public class SliderSetting
    {
        public string Name;
        public Slider Slider;
        public float Value;
        public float DefaultValue;
    }

    public static Dictionary<string, KeyCode> s_HotkeyMappings = new Dictionary<string, KeyCode>();
    public static Dictionary<string, float> s_SliderMappings = new Dictionary<string, float>(); // in game ref for slider settings
    public static event System.Action<string> OnSliderChanged;

    [SerializeField] private List<HotkeySetting> _hotkeySettings; // assigned in inspector
    [SerializeField] private List<SliderSetting> _sliderSettings; // assigned in inspector
    private KeyCode[] _allKeyCodes;
    private Dictionary<object, object> _uiSettingMapping = new Dictionary<object, object>(); // setting ui element, setting object
    private List<string> _usedKeys = new List<string>();
    private CanvasGroup _canvasGroup;
    private bool _waitingForInput = false;
    private string _newKey;

    private void Awake()
    {
        gameObject.SetActive(false);

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

        foreach (SliderSetting setting in _sliderSettings)
        {
            setting.Value = PlayerPrefs.GetFloat(setting.Name, setting.DefaultValue);
            setting.Slider.value = setting.Value;

            _uiSettingMapping.Add(setting.Slider, setting);

            setting.Slider.onValueChanged.AddListener(delegate { SliderChanged(setting.Slider); });
        }
    }

    // update playerprefs when menu closed
    private void OnDisable()
    {
        foreach (HotkeySetting setting in _hotkeySettings)
        {
            PlayerPrefs.SetString(setting.Name, setting.Key);

            // cache as in game reference for faster lookup
            // and since PlayerPrefs can't store KeyCode
            if (s_HotkeyMappings.Count == 0) // only on first update when empty
                s_HotkeyMappings.Add(setting.Name, GetKeyCode(setting.Key));
            else
                s_HotkeyMappings[setting.Name] = GetKeyCode(setting.Key);
        }

        foreach (SliderSetting setting in _sliderSettings)
        {
            PlayerPrefs.SetFloat(setting.Name, setting.Value);

            if (s_SliderMappings.Count == 0)
                s_SliderMappings.Add(setting.Name, setting.Value);
            else
                s_SliderMappings[setting.Name] = setting.Value;

            OnSliderChanged?.Invoke(setting.Name); // any updates to values in game only occur after settings menu closed
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
                    ErrorNotifier.Instance.GenerateErrorMsg(ErrorNotifier.ErrorMessages[ErrorMessageType.HotkeyConflict]);
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

    private void SliderChanged(Slider slider)
    {
        SliderSetting setting = (SliderSetting)_uiSettingMapping[slider];
        setting.Value = slider.value;
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
