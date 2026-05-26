using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    private const string SceneMain = "MainScene";
    private const string SceneSkill = "Skill";
    private const string SceneBattle = "GameScene";
    private const string SceneMonster = "Monster";

    [Header("�ε� �г�")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("���̵� �г�")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.4f;

    [Header("UI")]
    public UnitSo gameMonsterPanel;

    public bool IsTransitioning { get; private set; }

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(instance);
        DontDestroyOnLoad(gameObject);

        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0f;
        if (loadingPanel != null) loadingPanel.SetActive(false);
    }
    
    private void OnValidate()
    {
        if (fadeCanvasGroup == null)
            Debug.LogWarning("fadeCanvasGroup�� ����ֽ��ϴ�.");
        if (loadingPanel == null)
            Debug.LogWarning("loadingPanel�� ����ֽ��ϴ�.");
    }


    public void LoadMainScene() => LoadAsync(SceneMain).Forget();
    public void LoadGameScene() => LoadAsync(SceneBattle).Forget();
    public void LoadMonsterScene()
    {
        EnemyManager.instance.SaveSnapshot();
        MapManager.instance.Save();
        LoadAsync(SceneMonster).Forget();
    }
    public void LoadSkillScene()
    {
        EnemyManager.instance.SaveSnapshot();
        MapManager.instance.Save();
        LoadAsync(SceneSkill).Forget();
    }

    private async UniTask LoadAsync(string sceneName)
    {
        if (IsTransitioning) return;

        IsTransitioning = true;

        try
        {
            await FadeAsync(0f, 1f);

            SetLoadingUI(true, 0f);

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName); // �ε常
            op.allowSceneActivation = false; // 100% -> 0.9f

            while (op.progress < 0.9f)
            {
                SetLoadingUI(true, Mathf.Clamp01(op.progress / 0.9f));
                await UniTask.Yield();
            }

            SetLoadingUI(true, 1f);
            await UniTask.Delay(300);

            op.allowSceneActivation = true;
            await UniTask.WaitUntil(() => op.isDone);

            SetLoadingUI(false, 0f);
            await FadeAsync(1f, 0f);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"�� �ε� ���� ({sceneName}): {e.Message}");
            SetLoadingUI(false, 0f);
        }
        finally
        {
            IsTransitioning = false;
        }
    }

    private void SetLoadingUI(bool visible, float progress)
    {
        if (loadingPanel != null) loadingPanel.SetActive(visible);
        if (progressBar != null) progressBar.value = progress;
        if (progressText != null) progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
    }

    private async UniTask FadeAsync(float from, float to)
    {
        if (fadeCanvasGroup == null) return;

        float elapsed = 0f;
        fadeCanvasGroup.alpha = from;
        fadeCanvasGroup.blocksRaycasts = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            await UniTask.Yield();
        }

        fadeCanvasGroup.alpha = to;
        fadeCanvasGroup.blocksRaycasts = to > 0f;
    }

    
}
