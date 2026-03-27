using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Data/LevelDatabase")]
public class LevelDatabase : ScriptableObject
{
    public List<LevelData> allLevels;

    private Dictionary<int, LevelData> levelCache;
    public void Initialize()
    {
        if (levelCache != null) return;

        levelCache = new Dictionary<int, LevelData>(allLevels.Count);
        foreach (var level in allLevels)
        {
            int key = GetKey(level.dungeonGrade, level.stageNumber);
            if (!levelCache.ContainsKey(key))
            {
                levelCache.Add(key, level);
            }
        }
    }

    public LevelData GetLevelData(int grade, int stage)
    {
        if (levelCache == null)
        {
            Initialize();
        }

        int key = GetKey(grade, stage);
        if (levelCache.TryGetValue(key, out LevelData data))
        {
            return data;
        }

        Debug.LogError($"[LevelDatabase] 데이터 누락: Grade {grade}, Stage {stage}");
        return null;
    }

    private int GetKey(int grade, int stage) => (grade * 1000) + stage;
}