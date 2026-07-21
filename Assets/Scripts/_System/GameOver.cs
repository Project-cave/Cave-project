using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button btnRewindTime;
    [SerializeField] private Button btnGiveUp;

    private void Awake()
    {
        btnRewindTime.onClick.AddListener(OnRewindTime);
        btnGiveUp.onClick.AddListener(OnGiveUp);
    }

    public void TriggerGameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    private void OnRewindTime()
    {
        Debug.Log("계속 진행합니다.");
        gameOverPanel.SetActive(false);
        // 자원 감소 및 스테이지 업다운, 재시도 여부 
    }

    private void OnGiveUp()
    {
        Debug.Log("메인 메뉴로 돌아갑니다.");
        gameOverPanel.SetActive(false);
        SceneController.instance.LoadMainScene();
    }
}