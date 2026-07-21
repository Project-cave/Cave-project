using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    public int ResolutionIndex;
    public bool IsFullScreen;

    public float MasterVolume = 1f;
    public float MusicVolume = 0.6f;
    public float SfxVolume = 0.75f;

    [Header("UI")]
    [SerializeField] private GameObject SettingsContent;
    //[SerializeField] private Button homeBtn;
    [SerializeField] private GameObject Panel;

    private bool Open = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
            Debug.Log("�ʱ�ȭ �Ϸ�");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //public void GoLobby()
    //{
    //    ToggleSettings();
    //    SceneController.instance.LoadHomeScene();
    //}

    public void ToggleSettings()
    {
        bool activeSet = !SettingsContent.activeSelf;
        Panel.SetActive(activeSet);
        SettingsContent.SetActive(activeSet);
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

        if (AudioManager.instance == null) return;

        AudioManager.instance.SetMasterVolume(MasterVolume);
        AudioManager.instance.SetBgmVolume(MusicVolume);
        AudioManager.instance.SetSfxVolume(SfxVolume);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}