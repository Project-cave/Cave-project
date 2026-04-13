using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("데이터베이스 및 풀")]
    [SerializeField] private LevelDatabase levelDatabase;
    [SerializeField] private Transform spawnPoint;

    [Header("소환 설정")]
    [SerializeField] private float spawnDelay = 5.0f;

    [Header("랜덤 풀(Pool) 데이터")]
    public List<RaceData> raceList;
    public List<RankData> rankList;
    public List<ClassData> classList;

    // 탐색을 위한 딕셔너리
    private Dictionary<EnemyRaceType, RaceData> raceCache;
    private Dictionary<EnemyClassType, ClassData> classCache;
    private Dictionary<EnemyRankType, RankData> rankCache;

    // 소환 간격 관리용
    private Coroutine currentSpawnCoroutine;

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

    public void StartLevelSpawning(int grade, int stage)
    {
        if (currentSpawnCoroutine != null)
        {
            StopCoroutine(currentSpawnCoroutine);
        }

        currentSpawnCoroutine = StartCoroutine(SpawnEnemiesCoroutine(grade, stage));
    }

    private IEnumerator SpawnEnemiesCoroutine(int grade, int stage)
    {


        yield return new WaitForSeconds(spawnDelay);

        if (levelDatabase == null)
        {
            Debug.LogError("[StageEnemySpawner] LevelDatabase가 할당되지 않음.");
            yield break;
        }

        LevelData data = levelDatabase.GetLevelData(grade, stage);
        if (data == null) yield break;

        yield return StartCoroutine(SpawnEnemyCourutine(EnemyRankType.Bronze, data.bronzeCount, data));
        yield return StartCoroutine(SpawnEnemyCourutine(EnemyRankType.Silver, data.silverCount, data));
        yield return StartCoroutine(SpawnEnemyCourutine(EnemyRankType.Gold, data.goldCount, data));
        yield return StartCoroutine(SpawnEnemyCourutine(EnemyRankType.Platinum, data.platinumCount, data));

        currentSpawnCoroutine = null;
    }

    private IEnumerator SpawnEnemyCourutine(EnemyRankType rankType, int count, LevelData data)
    {
        if (count <= 0) yield break;

        if (!rankCache.TryGetValue(rankType, out RankData targetRankData))
        {
            Debug.LogError($"[StageEnemySpawner] {rankType} 랭크 데이터 누락.");
            yield break;
        }

        for (int i = 0; i < count; i++)
        {
            EnemyRaceType race = GetRandomRace(data);
            EnemyClassType enemyClass = GetRandomClass(data);

            if (!raceCache.TryGetValue(race, out RaceData targetRaceData))
            {
                Debug.LogError($"[StageEnemySpawner] {race} 종족 데이터 누락.");
                continue;
            }
            if (!classCache.TryGetValue(enemyClass, out ClassData targetClassData))
            {
                Debug.LogError($"[StageEnemySpawner] {enemyClass} 직업 데이터 누락.");
                continue;
            }

            InitializeEnemy(targetRankData, targetRaceData, targetClassData);

            yield return new WaitForSeconds(spawnDelay);
        }
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

    private void InitializeEnemy(RankData rankData, RaceData raceData, ClassData classData)
    {
        GameObject spawnedEnemy = GameManager.instance.pool.Get(2);
        spawnedEnemy.transform.position = spawnPoint.position;

        EnemyStatHandler statHandler = spawnedEnemy.GetComponent<EnemyStatHandler>();
        if (statHandler != null)
        {
            statHandler.raceData = raceData;
            statHandler.rankData = rankData;
            statHandler.classData = classData;
        }

        Enemy existingScript = spawnedEnemy.GetComponent<Enemy>();
        if (existingScript != null) DestroyImmediate(existingScript);

        Enemy newEnemyScript = null;
        if (classData.attackType == EnemyAttackType.Melee) newEnemyScript = spawnedEnemy.AddComponent<EnemyMeleeClass>();
        else if (classData.attackType == EnemyAttackType.Ranged) newEnemyScript = spawnedEnemy.AddComponent<EnemyRangedClass>();

        SpriteRenderer sr = spawnedEnemy.GetComponent<SpriteRenderer>();
        if (sr != null && rankData != null)
        {
            switch (rankData.rankType)
            {
                case EnemyRankType.Bronze:
                    sr.color = Color.white;
                    break;
                case EnemyRankType.Silver:
                    sr.color = new Color(0.8f, 0.8f, 0.9f); // 은빛
                    break;
                case EnemyRankType.Gold:
                    sr.color = new Color(1.0f, 0.9f, 0.4f); // 금빛
                    break;
                case EnemyRankType.Platinum:
                    sr.color = new Color(0.8f, 1.0f, 1.0f); // 빛나는 하늘색
                    break;
            }
        }

        if (newEnemyScript != null)
        {
            newEnemyScript.InitEnemy();
        }
    }
}