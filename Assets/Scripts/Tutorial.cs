using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [Header("Tutorial Story SO")]
    [SerializeField] private StoryData openingStory;

    [Header("External Data")]
    [SerializeField] private CutsceneManager cutsceneManager;

    [Header("Systems to Control")]
    [SerializeField] private GameObject spawnStage;

    private int currentStep = 0;

    private void Start()
    {
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 1)
        {
            Debug.Log("튜토리얼 스킵.");
            if (spawnStage != null) spawnStage.SetActive(true);
            gameObject.SetActive(false);
            return;
        }

        if (spawnStage != null) spawnStage.SetActive(false);

        StartOpening();
    }

    private void StartOpening()
    {
        cutsceneManager.PlayStory(openingStory, () => 
        {
            currentStep = 1;
            // 다음 튜토리얼
            FinishTutorial();
        });
    }

    public void FinishTutorial()
    {
        Debug.Log("튜토리얼 완료.");
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();

        if (spawnStage != null) spawnStage.SetActive(true);

        gameObject.SetActive(false);
    }
}