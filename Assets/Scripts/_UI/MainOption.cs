using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainOption : MonoBehaviour
{
    public static MainOption instance = null;

    [Header("패널")]
    public GameObject UIWindow = null;

    [Header("탭 패널")]
    public GameObject gamePanel;
    public GameObject videoPanel;
    public GameObject audioPanel;

    public Button gamePanelBtn;
    public Button videoPanelBtn;
    public Button audioPanelBtn;

    [Header("비디오")]
    public TextMeshProUGUI screenModeLabel;
    private int _screenModeIndex = 0;
    private readonly string[] _screenModeNames = { "전체화면", "창모드" };

    public TextMeshProUGUI resolutionLabel;
    private int _resolutionIndex = 0;
    private readonly string[] _resolutionNames = { "1920 x 1080", "1600 x 900", "1280 x 720" };

    [Header("사운드")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public TextMeshProUGUI masterlabel;
    public TextMeshProUGUI bgmLabel;
    public TextMeshProUGUI sfxLabel;

    private bool _isUpdatingSlider = false;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        InitSliderListeners();

        if (SettingsManager.instance != null) PullFromSettings();

        RefreshLabel();
        ShowTab(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsPanel();
        }
    }

    private void InitSliderListeners()
    {
        if (masterSlider)
        {
            masterSlider.minValue = 0f;
            masterSlider.maxValue = 1f;
            masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        }
        if (bgmSlider)
        {
            bgmSlider.minValue = 0f;
            bgmSlider.maxValue = 1f;
            bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        }
        if (sfxSlider)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        }
    }

    private void PullFromSettings()
    {
        var sm = SettingsManager.instance;

        if (sm == null) return;

        _screenModeIndex = sm.IsFullScreen ? 0 : 1;
        _resolutionIndex = sm.ResolutionIndex;

        _isUpdatingSlider = true;
        if (masterSlider) masterSlider.value = sm.MasterVolume;
        if (bgmSlider) bgmSlider.value = sm.MusicVolume;
        if (sfxSlider) sfxSlider.value = sm.SfxVolume;
        _isUpdatingSlider = false;
    }

    public void ToggleSettingsPanel()
    {
        if (UIWindow == null) return;
        bool open = !UIWindow.activeSelf;
        UIWindow.SetActive(open);

        if (open)
        {
            PullFromSettings();
            RefreshLabel();
        }
        else
            SettingsManager.instance.SaveSettings();
    }

    public void ShowTab(int index)
    {
        gamePanel?.SetActive(index == 0);
        videoPanel?.SetActive(index == 1);
        audioPanel?.SetActive(index == 2);
    }

    private void RefreshLabel()
    {
        if (screenModeLabel) screenModeLabel.text = _screenModeNames[_screenModeIndex];
        if (resolutionLabel) resolutionLabel.text = _resolutionNames[_resolutionIndex];

        if (masterSlider && masterlabel)
            masterlabel.text = Mathf.RoundToInt(masterSlider.value * 100) + "%";
        if (bgmSlider && bgmLabel)
            bgmLabel.text = Mathf.RoundToInt(bgmSlider.value * 100) + "%";
        if (sfxSlider && sfxLabel)
            sfxLabel.text = Mathf.RoundToInt(sfxSlider.value * 100) + "%";
    }

    public void ScreenModePrev() => CycleScreenMode(-1);
    public void ScreenModNext() => CycleScreenMode(1);

    private void CycleScreenMode(int dir)
    {
        _screenModeIndex = (_screenModeIndex + dir + _screenModeNames.Length) % _screenModeNames.Length;
        ApplyScreenMode();
        if (screenModeLabel) screenModeLabel.text = _screenModeNames[_screenModeIndex];
    }

    private void ApplyScreenMode()
    {
        bool full = _screenModeIndex == 0;
        Screen.fullScreenMode = full ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        SettingsManager.instance.IsFullScreen = full;
    }

    public void ResolutionPrev() => CycleResolution(-1);
    public void ResolutionNext() => CycleResolution(+1);

    private void CycleResolution(int dir)
    {
        _resolutionIndex = (_resolutionIndex + dir + _resolutionNames.Length) % _resolutionNames.Length;
        ApplyResolution();
        if (resolutionLabel) resolutionLabel.text = _resolutionNames[_resolutionIndex];

    }

    private void ApplyResolution()
    {
        SettingsManager.instance.ResolutionIndex = _resolutionIndex;
        switch (_resolutionIndex)
        {
            case 0: Screen.SetResolution(1920, 1080, Screen.fullScreenMode); break;
            case 1: Screen.SetResolution(1600, 900, Screen.fullScreenMode); break;
            case 2: Screen.SetResolution(1280, 720, Screen.fullScreenMode); break;
        }
    }


    private void OnMasterSliderChanged(float value)
    {
        if (_isUpdatingSlider) return;
        AudioManager.instance.SetMasterVolume(value);
        SettingsManager.instance.MasterVolume = value;
        if (masterlabel) masterlabel.text = Mathf.RoundToInt(value * 100) + "%";

    }

    private void OnBgmSliderChanged(float value)
    {
        if (_isUpdatingSlider) return;
        AudioManager.instance.SetBgmVolume(value);
        SettingsManager.instance.MusicVolume = value;
        if (bgmLabel) bgmLabel.text = Mathf.RoundToInt(value * 100) + "%";

    }

    private void OnSfxSliderChanged(float value)
    {
        if (_isUpdatingSlider) return;
        AudioManager.instance.SetSfxVolume(value);
        SettingsManager.instance.SfxVolume = value;
        if (sfxLabel) sfxLabel.text = Mathf.RoundToInt(value * 100) + "%";

    }
}