using UnityEngine;
using System.Collections;

public class RaidManager : MonoBehaviour
{
    public static RaidManager Instance { get; private set; }
    
    [Header("Raid Locations")]
    [SerializeField] private RaidData[] raidLocations;
    
    private bool isRaidActive = false;
    private float currentRaidTime = 0f;
    private float targetRaidTime = 0f;
    private RaidData currentRaid = null;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public RaidData[] GetRaidLocations()
    {
        return raidLocations;
    }
    
    public bool IsRaidActive()
    {
        return isRaidActive;
    }
    
    public float GetRaidProgress()
    {
        if (!isRaidActive) return 0f;
        return currentRaidTime / targetRaidTime;
    }
    
    public void StartRaid(RaidData raid)
    {
        if (isRaidActive)
        {
            Debug.Log("이미 약탈 중입니다!");
            return;
        }
        
        currentRaid = raid;
        StartCoroutine(RaidCoroutine(raid));
    }
    
    IEnumerator RaidCoroutine(RaidData raid)
    {
        isRaidActive = true;
        currentRaidTime = 0f;
        targetRaidTime = raid.raidDuration;
        
        Debug.Log($"{raid.GetLocationName()} ({raid.GetDifficultyName()}) 약탈 시작! (시간: {raid.raidDuration}초, 성공률: {raid.baseSuccessRate}%)");
        
        while (currentRaidTime < targetRaidTime)
        {
            currentRaidTime += Time.deltaTime;
            yield return null;
        }
        
        float roll = Random.Range(0f, 100f);
        bool success = roll <= raid.baseSuccessRate;
        
        if (success)
        {
            foreach (var reward in raid.possibleRewards)
            {
                int amount = Random.Range(reward.minAmount, reward.maxAmount + 1);
                ResourceManager.Instance.AddResource(reward.resourceType, amount);
                Debug.Log($"{GetResourceName(reward.resourceType)} +{amount}");
            }
            
            Debug.Log($"{raid.GetLocationName()} 약탈 성공!");
            
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowRaidResult(true, raid);
            }
        }
        else
        {
            Debug.Log($"{raid.GetLocationName()} 약탈 실패!");
            
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowRaidResult(false, raid);
            }
        }
        
        isRaidActive = false;
        currentRaidTime = 0f;
        currentRaid = null;
    }
    
    string GetResourceName(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Wood: return "나무";
            case ResourceType.Scrap: return "고철";
            case ResourceType.Stone: return "돌";
            default: return "";
        }
    }
}