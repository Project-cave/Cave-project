using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("데이터베이스 및 풀")]
    [SerializeField] private LevelDatabase levelDatabase;

    [Header("소환 설정")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float gatherWaitTime = 2.0f;

    [Header("랜덤 풀(Pool) 데이터")]
    public List<RaceData> raceList;
    public List<RankData> rankList;
    public List<ClassData> classList;

    // 탐색을 위한 딕셔너리
    private Dictionary<EnemyRaceType, RaceData> raceCache;
    private Dictionary<EnemyClassType, ClassData> classCache;
    private Dictionary<EnemyRankType, RankData> rankCache;

    private void Awake()
    {
        InitializeCache();
    }

    private void InitializeCache()
    {
        raceCache = new Dictionary<EnemyRaceType, RaceData>(raceList.Count);
        foreach (var race in raceList) raceCache[race.raceType] = race;

        classCache = new Dictionary<EnemyClassType, ClassData>(classList.Count);
        foreach (var c in classList) classCache[c.classType] = c;

        rankCache = new Dictionary<EnemyRankType, RankData>(rankList.Count);
        foreach (var rank in rankList) rankCache[rank.rankType] = rank;
    }

    public void StartWaveSpawning(int dungeonRank, int dungeonWave, bool isBoss)
    {
        StartCoroutine(SpawnAndGatherCoroutine(dungeonRank, dungeonWave, isBoss));
    }

    private IEnumerator SpawnAndGatherCoroutine(int dungeonRank, int dungeonWave, bool isBoss)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[EnemySpawner] spawnPoints가 설정되지 있지 않습니다.");
            yield break;
        }

        LevelData data = levelDatabase != null ? levelDatabase.GetLevelData(dungeonRank, dungeonWave) : null;
        int spawnCount = GetRandomSpawnCount(data);

        List<Enemy> currentWaveEnemies = new List<Enemy>();

        for (int i = 0; i < spawnCount; i++)
        {
            Transform selectedPoint = spawnPoints[i];

            Enemy newEnemy = SpawnSingleEnemy(isBoss, selectedPoint.position, data);

            if (newEnemy != null)
            {
                newEnemy.SetMoveable(false);
                currentWaveEnemies.Add(newEnemy);
            }
        }

        Debug.Log($"[EnemySpawner] {spawnCount}명의 모험가가 대기 중입니다.");
        yield return new WaitForSeconds(gatherWaitTime);

        Debug.Log("[EnemySpawner] 모험가가 이동을 시작했습니다.");
        foreach (var enemy in currentWaveEnemies)
        {
            if (enemy != null)
            {
                enemy.SetMoveable(true);
                if (EnemyManager.instance != null)
                {
                    EnemyManager.instance.RegisterEnemy(enemy.gameObject);
                }
            }
        }

        StageManager.instance.isWaveActive = true;
    }

    private int GetRandomSpawnCount(LevelData level)
    {
        return Random.Range(level.minSpawn, level.maxSpawn + 1);
    }

    private Enemy SpawnSingleEnemy(bool isBoss, Vector3 spawnPos, LevelData data)
    {
        EnemyRankType targetRankType = GetRandomRank(data);
        if (!rankCache.TryGetValue(targetRankType, out RankData rankData)) return null;

        EnemyRaceType targetRaceType = GetRandomRace(data);
        if (!raceCache.TryGetValue(targetRaceType, out RaceData raceData)) return null;

        EnemyClassType targetClassType = GetRandomClass(data);
        if (!classCache.TryGetValue(targetClassType, out ClassData classData)) return null;

        GameObject spawnedEnemy = GameManager.instance.pool.Get(12);
        spawnedEnemy.transform.position = spawnPos;

        EnemyStatHandler statHandler = spawnedEnemy.GetComponent<EnemyStatHandler>();
        if (statHandler != null)
        {
            statHandler.raceData = raceData;
            statHandler.rankData = rankData;
            statHandler.classData = classData;
        }

        Enemy newEnemyScript = null;
        if (classData.attackType == EnemyAttackType.Melee) newEnemyScript = spawnedEnemy.AddComponent<EnemyMeleeClass>();
        else if (classData.attackType == EnemyAttackType.Ranged) newEnemyScript = spawnedEnemy.AddComponent<EnemyRangedClass>();

        SpriteRenderer sr = spawnedEnemy.GetComponent<SpriteRenderer>();
        if (sr != null && rankData != null)
        {
            switch (rankData.rankType)
            {
                case EnemyRankType.Bronze: sr.color = Color.white; break;
                case EnemyRankType.Silver: sr.color = new Color(0.8f, 0.8f, 0.9f); break;
                case EnemyRankType.Gold: sr.color = new Color(1.0f, 0.9f, 0.4f); break;
                case EnemyRankType.Platinum: sr.color = new Color(0.8f, 1.0f, 1.0f); break;
            }
        }

        if (newEnemyScript != null)
        {
            newEnemyScript.InitEnemy();
        }

        return newEnemyScript;
    }

    private EnemyRankType GetRandomRank(LevelData data)
    {
        float total = data.bronzeProb + data.silverProb + data.goldProb + data.platinumProb;
        float rand = Random.Range(0f, total);

        if ((rand -= data.bronzeProb) < 0) return EnemyRankType.Bronze;
        if ((rand -= data.silverProb) < 0) return EnemyRankType.Silver;
        if ((rand -= data.goldProb) < 0) return EnemyRankType.Gold;

        return EnemyRankType.Platinum;
    }

    private EnemyRaceType GetRandomRace(LevelData data)
    {
        float total = data.humanProb + data.elfProb + data.dwarfProb + data.anthroProb;
        float rand = Random.Range(0f, total);

        if ((rand -= data.humanProb) < 0) return EnemyRaceType.Human;
        if ((rand -= data.elfProb) < 0) return EnemyRaceType.Elf;
        if ((rand -= data.dwarfProb) < 0) return EnemyRaceType.Dwarf;

        return EnemyRaceType.Anthro;
    }

    private EnemyClassType GetRandomClass(LevelData data)
    {
        float total = data.warriorProb + data.archerProb + data.knightProb +
                      data.swordsManProb + data.assassinProb + data.wizardProb +
                      data.magicianProb + data.paladinProb;
        float rand = Random.Range(0f, total);

        if ((rand -= data.warriorProb) < 0) return EnemyClassType.Warrior;
        if ((rand -= data.archerProb) < 0) return EnemyClassType.Archer;
        if ((rand -= data.knightProb) < 0) return EnemyClassType.Knight;
        if ((rand -= data.swordsManProb) < 0) return EnemyClassType.SwordsMan;
        if ((rand -= data.assassinProb) < 0) return EnemyClassType.Assasin;
        if ((rand -= data.wizardProb) < 0) return EnemyClassType.Wizard;
        if ((rand -= data.magicianProb) < 0) return EnemyClassType.Magician;

        return EnemyClassType.Paladin;
    }
}