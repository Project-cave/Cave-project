using UnityEngine;
using System;

public enum ResourceType
{
    Wood,   // 나무
    Scrap,  // 고철
    Stone   // 돌
}

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    
    [Header("Starting Resources")]
    [SerializeField] private int startingWood = 50;
    [SerializeField] private int startingScrap = 30;
    [SerializeField] private int startingStone = 20;
    
    private int currentWood;
    private int currentScrap;
    private int currentStone;
    
    public event Action OnResourceChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        currentWood = startingWood;
        currentScrap = startingScrap;
        currentStone = startingStone;
        OnResourceChanged?.Invoke();
    }
    
    public int GetResource(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Wood: return currentWood;
            case ResourceType.Scrap: return currentScrap;
            case ResourceType.Stone: return currentStone;
            default: return 0;
        }
    }
    
    public bool CanAfford(ResourceType type, int cost)
    {
        return GetResource(type) >= cost;
    }
    
    public bool SpendResource(ResourceType type, int amount)
    {
        if (!CanAfford(type, amount))
            return false;
        
        switch (type)
        {
            case ResourceType.Wood:
                currentWood -= amount;
                break;
            case ResourceType.Scrap:
                currentScrap -= amount;
                break;
            case ResourceType.Stone:
                currentStone -= amount;
                break;
        }
        
        OnResourceChanged?.Invoke();
        return true;
    }
    
    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                currentWood += amount;
                break;
            case ResourceType.Scrap:
                currentScrap += amount;
                break;
            case ResourceType.Stone:
                currentStone += amount; 
                break;
        }
        
        OnResourceChanged?.Invoke();
    }
    
    // 이전 버전 호환성 유지 (ScoutManager용)
    public int GetCurrentResources()
    {
        return currentWood + currentScrap + currentStone;
    }
    
    public void AddResources(int amount)
    {
        // 임시: 나무에만 추가
        currentWood += amount;
        OnResourceChanged?.Invoke();
    }
}
