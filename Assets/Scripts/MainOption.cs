using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainOption : MonoBehaviour
{
    public GameObject UIWindow = null;

    [Header("사운드")]
    public GameObject bgmVolumeImage;
    public GameObject bgmVolumeMuteImage;
    public GameObject sfxVolumeImage;
    public GameObject sfxVolumeMuteImage;

    [Header("환경 설정 UI")]
    public TMP_Dropdown IsFullscreen;
    public TMP_Dropdown Resolution;
    public Slider Music;
    public Slider Sfx;

    private void Start()
    {
        SettingsManager.instance.LoadSettings();
        UpdateUI();
    }

    public void OpenUI()
    {
        if (UIWindow)
        {
            UIWindow.SetActive(true);
        }
    }

    public void CloseUI()
    {
        if (UIWindow)
        {
            Debug.Log("Close");
            UIWindow.SetActive(false);
        }
    }

    public void ChangeConfirm()
    {
        SettingsManager.instance.SaveSettings();
    }

    public void UpdateUI()
    {
        IsFullscreen.value = (PlayerPrefs.GetInt("IsFullscreen") == 1 ? 0 : 1);
        Resolution.value = PlayerPrefs.GetInt("ResolutionIndex");
        Music.value = PlayerPrefs.GetFloat("MusicVolume");
        Sfx.value = PlayerPrefs.GetFloat("SfxVolume");
    }

    public void ChangeScreenMode(int index) // 화면 모드 전환
    {
        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                SettingsManager.instance.IsFullScreen = true;
                Debug.Log("전체화면");
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                SettingsManager.instance.IsFullScreen = false;
                Debug.Log("창모드");
                break;
            default:
                Debug.Log("Error");
                break;
        }
    }

    public void ChangeResolution(int index) // 화면 해상도 조절
    {
        switch (index)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
                SettingsManager.instance.ResolutionIndex = index;
                break;
            case 1:
                Screen.SetResolution(1600, 900, Screen.fullScreenMode);
                SettingsManager.instance.ResolutionIndex = index;
                break;
            case 2:
                Screen.SetResolution(1280, 720, Screen.fullScreenMode);
                SettingsManager.instance.ResolutionIndex = index;
                break;
            default:
                break;
        }
    }

    public void SetBgmVolume(float volume) // 배경음 조절
    {
        AudioManager.instance.SetBgmVolume(volume);
        SettingsManager.instance.MusicVolume = volume;
    }

    public void SetSfxVolume(float volume) // 효과음 조절
    {
        AudioManager.instance.SetSfxVolume(volume);
        SettingsManager.instance.SfxVolume = volume;
    }

    public void FBgmButton()
    {
        if (!AudioManager.instance.bgmVolumeMute) // 음소거 상태 아닐때
        {
            //bgmVolumeImage.SetActive(false);
            //bgmVolumeMuteImage.SetActive(true);
        }
        else // 음소거 상태
        {
            //bgmVolumeImage.SetActive(true);
            //bgmVolumeMuteImage.SetActive(false);
        }
        AudioManager.instance.bgmVolumeMute = !AudioManager.instance.bgmVolumeMute;
        SetBgmVolume(AudioManager.instance.bgmVolume);
    }

    public void FsfxButton()
    {
        if (!AudioManager.instance.sfxVolumeMute) // 음소거 상태 아닐때
        {
            //sfxVolumeImage.SetActive(false);
            //sfxVolumeMuteImage.SetActive(true);
        }
        else // 음소거 상태
        {
            //sfxVolumeImage.SetActive(true);
            //sfxVolumeMuteImage.SetActive(false);
        }
        AudioManager.instance.sfxVolumeMute = !AudioManager.instance.sfxVolumeMute;
        SetSfxVolume(AudioManager.instance.sfxVolume);
    }

    public void SaveSet()
    {
        SettingsManager.instance.SaveSettings();
    }

    public void LoadSet()
    {
        SettingsManager.instance.LoadSettings();
        ChangeResolution(SettingsManager.instance.ResolutionIndex);
        ChangeScreenMode(SettingsManager.instance.IsFullScreen ? 0 : 1);
        UpdateUI();
    }
}
