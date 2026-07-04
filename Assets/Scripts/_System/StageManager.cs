using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    [Header("참조")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private LevelDatabase levelDatabase;

    [Header("웨이브 상태")]
    public bool isWaveActive = false;
    public bool isCurrentWaveBoss = false;

    [Header("진행 상태")]
    public int currentDungeonRank = 1;
    public int currentWaveCount = 1;

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
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (enemySpawner == null || levelDatabase == null) return;
        if (!enemySpawner.gameObject.activeInHierarchy) enemySpawner.gameObject.SetActive(true);

        LevelData currentLevelData = levelDatabase.GetLevelData(currentDungeonRank, currentWaveCount);
        if (currentLevelData == null)
        {
            Debug.LogError($"[StageManager] 현재 LevelData가 존재하지 않습니다.");
            return;
        }

        isCurrentWaveBoss = (currentWaveCount >= currentLevelData.targetWaveCount);
        isWaveActive = false;

        if (isCurrentWaveBoss)
        {
            Debug.Log($"[StageManager] {currentDungeonRank}등급 보스 웨이브.");
            enemySpawner.StartWaveSpawning(currentDungeonRank, currentWaveCount, true);
        }
        else
        {
            Debug.Log($"[StageManager] {currentDungeonRank}등급 일반 웨이브 ({currentWaveCount}/{currentLevelData.targetWaveCount}).");
            enemySpawner.StartWaveSpawning(currentDungeonRank, currentWaveCount, false);
        }
    }

    public void OnWaveCleared(bool wasBoss)
    {
        if (wasBoss)
        {
            currentDungeonRank++;
            currentWaveCount = 1;
        }
        else
        {
            currentWaveCount++;
        }

        Invoke(nameof(StartNextWave), 3.0f);
    }

    public void OnWaveFailed()
    {

        currentWaveCount = 1;
    }
}