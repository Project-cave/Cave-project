using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu Panels")]
    [SerializeField] private GameObject titlePanel;
    /*[SerializeField] private GameObject optionPopup;
    [SerializeField] private GameObject saveLoadPopup;
    [SerializeField] private GameObject memoryPanel;

    [Header("Buttons")]
    [SerializeField] private Button btnNewGame;
    [SerializeField] private Button btnLoadGame;
    [SerializeField] private Button btnOption;
    [SerializeField] private Button btnMemory;*/

    private void Awake()
    {
        titlePanel.SetActive(true);

        /*
        btnNewGame.onClick.AddListener(StartNewGameTutorial);
        btnLoadGame.onClick.AddListener(() => saveLoadPopup.SetActive(true));
        btnOption.onClick.AddListener(() => optionPopup.SetActive(true));
        btnMemory.onClick.AddListener(() => memoryPanel.SetActive(true));
        */
    }

    private void Update()
    {
        if (titlePanel.activeSelf && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            StartNewGameTutorial();
        }
    }

    private void StartNewGameTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();

        Debug.Log("메인화면 종료.");
        SceneController.instance.LoadGameScene();
    }
}