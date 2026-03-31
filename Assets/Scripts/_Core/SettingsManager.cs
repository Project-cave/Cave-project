using System;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    public int ResolutionIndex;
    public bool IsFullScreen;


    public float MasterVolume = 1f;
    public float MusicVolume = 0.2f;
    public float SfxVolume = 0.2f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
            Debug.Log("초기화 완료");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionIndex", ResolutionIndex);
        PlayerPrefs.SetInt("IsFullscreen", IsFullScreen ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        ResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 2);
        IsFullScreen = PlayerPrefs.GetInt("IsFullscreen", 1) == 1;
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
        SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.75f);
        ApplySettings();
    }

    public void ApplySettings()
    {
        AudioManager.instance.SetMasterVolume(MasterVolume);
        AudioManager.instance.SetBgmVolume(MusicVolume);
        AudioManager.instance.SetSfxVolume(SfxVolume);
    }
}
