using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 타이틀 화면 "게임 시작" 버튼.
/// 클릭 -> 배경을 라이팅 버전 스프라이트로 교체 -> 잠깐 보여줌 -> 씬 전환.
///
/// 인스펙터 세팅:
///   backgroundImage  : 배경을 표시 중인 UI Image 컴포넌트
///   normalSprite     : mainbackground.png
///   litSprite        : mainbackground_lighting.png
///   delayBeforeLoad  : 라이팅 이미지를 보여줄 시간(초)
///   gameSceneName    : 전환할 씬 이름
/// </summary>
public class StartGameButton : MonoBehaviour
{
    [Header("배경 이미지")]
    public Image backgroundImage;
    public Sprite normalSprite;
    public Sprite litSprite;

    [Header("타이밍")]
    [Tooltip("라이팅 이미지로 바뀐 뒤 씬 전환까지 대기 시간(초)")]
    public float delayBeforeLoad = 1.5f;

    [Header("씬 전환")]
    public string gameSceneName = "GameScene";

    [Header("버튼 (중복 클릭 방지용, 선택)")]
    public Button button;

    private bool isTransitioning = false;

    public void StartGame()
    {
        if (isTransitioning) return; // 연타 방지
        isTransitioning = true;

        if (button != null)
            button.interactable = false;

        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        // 배경을 라이팅 버전으로 교체
        if (backgroundImage != null && litSprite != null)
            backgroundImage.sprite = litSprite;

        yield return new WaitForSeconds(delayBeforeLoad);

        SceneManager.LoadScene(gameSceneName);
    }
}