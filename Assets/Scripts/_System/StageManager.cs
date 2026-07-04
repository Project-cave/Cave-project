using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    [Header("참조")]
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("진행 상태")]
    public int currentGrade = 1;
    public int currentStage = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StartStage();
    }

    public void StartStage()
    {
        if (enemySpawner == null)
        {
            Debug.LogError("[StageManager] EnemySpawner 참조가 누락");
            return;
        }

        if (!enemySpawner.gameObject.activeInHierarchy)
        {
            enemySpawner.gameObject.SetActive(true);
        }

        Debug.Log($"[StageManager] {currentGrade}등급 {currentStage}스테이지.");
        enemySpawner.StartLevelSpawning(currentGrade, currentStage);
    }

    public void StageClear()
    {  
        // 스테이지 클리어 시
    }
}