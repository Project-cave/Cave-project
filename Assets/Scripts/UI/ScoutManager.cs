using UnityEngine;
using System.Collections;

public class ScoutManager : MonoBehaviour
{
    public static ScoutManager Instance { get; private set; }
    
    [Header("Settings")]
    [SerializeField] private float scoutDuration = 5f;
    [SerializeField] private int minReward = 1;
    [SerializeField] private int maxReward = 10;
    
    private bool isScoutActive = false;
    private float currentScoutTime = 0f;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public bool IsScoutActive()
    {
        return isScoutActive;
    }
    
    public float GetScoutProgress()
    {
        if (!isScoutActive) return 0f;
        return currentScoutTime / scoutDuration;
    }
    
    public void StartScout()
    {
        if (isScoutActive)
        {
            Debug.Log("이미 정찰 중입니다!");
            return;
        }
        
        StartCoroutine(ScoutCoroutine());
    }
    
    IEnumerator ScoutCoroutine()
    {
        isScoutActive = true;
        currentScoutTime = 0f;
        
        Debug.Log("정찰 시작!");
        
        // 5초 대기 (진행 시간 추적)
        while (currentScoutTime < scoutDuration)
        {
            currentScoutTime += Time.deltaTime;
            yield return null;
        }
        
        // 랜덤 자원 획득 (1~10)
        int reward = Random.Range(minReward, maxReward + 1);
        ResourceManager.Instance.AddResources(reward);
        
        Debug.Log($"정찰 완료! 자원 +{reward}");
        
        isScoutActive = false;
        currentScoutTime = 0f;
        
        // UI에 결과 표시
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowScoutResult(reward);
        }
    }
}
