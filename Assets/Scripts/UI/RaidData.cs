using UnityEngine;

[System.Serializable]
public class ResourceReward
{
    public ResourceType resourceType;
    public int minAmount;
    public int maxAmount;
}

public enum RaidDifficulty
{
    Easy,    // 쉬움
    Normal,  // 보통
    Hard     // 어려움
}

[CreateAssetMenu(fileName = "NewRaidLocation", menuName = "Tower Defense/Raid Location Data")]
public class RaidData : ScriptableObject
{
    [Header("Location Info")]
    public int villageId;  // 1, 2, 3
    public RaidDifficulty difficulty;
    
    [Header("Rewards")]
    public ResourceReward[] possibleRewards;
    
    [Header("Raid Settings")]
    [Range(0f, 100f)]
    public float baseSuccessRate = 70f;
    public float raidDuration = 10f;
    
    // 표시용 이름 생성
    public string GetLocationName()
    {
        return $"마을 {villageId}";
    }
    
    public string GetDifficultyName()
    {
        switch (difficulty)
        {
            case RaidDifficulty.Easy: return "쉬움";
            case RaidDifficulty.Normal: return "보통";
            case RaidDifficulty.Hard: return "어려움";
            default: return "";
        }
    }
}