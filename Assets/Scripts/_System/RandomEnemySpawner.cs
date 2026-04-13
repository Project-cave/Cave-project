using System.Collections.Generic;
using UnityEngine;

public class RandomEnemySpawner : MonoBehaviour
{
    [Header("소환 설정")]
    public Transform spawnPoint;
    public float spawnInterval = 3f;
    private float timer = 0f;

    [Header("랜덤 풀(Pool) 데이터")]
    public List<RaceData> raceList;
    public List<RankData> rankList;
    public List<ClassData> classList;

    private void Awake()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnRandomEnemy();
            timer = 0f;
        }
    }

    private void SpawnRandomEnemy()
    {
        if (raceList.Count == 0 || rankList.Count == 0 || classList.Count == 0)
        {
            Debug.LogWarning("스포너에 데이터 리스트가 비어있습니다!");
            return;
        }

        RaceData randomRace = raceList[Random.Range(0, raceList.Count)];
        RankData randomRank = rankList[Random.Range(0, rankList.Count)];
        ClassData randomClass = classList[Random.Range(0, classList.Count)];

        GameObject spawnedEnemy = GameManager.instance.pool.Get(2);
        spawnedEnemy.transform.position = spawnPoint.position;

        EnemyStatHandler statHandler = spawnedEnemy.GetComponent<EnemyStatHandler>();
        if (statHandler != null)
        {
            statHandler.raceData = randomRace;
            statHandler.rankData = randomRank;
            statHandler.classData = randomClass;
        }

        Enemy existingScript = spawnedEnemy.GetComponent<Enemy>();
        if (existingScript != null)
        {
            DestroyImmediate(existingScript);
        }

        if (randomClass.attackType == EnemyAttackType.Melee)
        {
            spawnedEnemy.AddComponent<EnemyMeleeClass>();
        }
        else if (randomClass.attackType == EnemyAttackType.Ranged)
        {
            spawnedEnemy.AddComponent<EnemyRangedClass>();
        }

        SpriteRenderer sr = spawnedEnemy.GetComponent<SpriteRenderer>();
        if (sr != null && randomRank != null)
        {
            switch (randomRank.rankType)
            {
                case EnemyRankType.Bronze:
                    sr.color = Color.white;
                    spawnedEnemy.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case EnemyRankType.Silver:
                    sr.color = new Color(0.8f, 0.8f, 0.9f); // 은빛
                    spawnedEnemy.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                    break;
                case EnemyRankType.Gold:
                    sr.color = new Color(1.0f, 0.9f, 0.4f); // 금빛
                    spawnedEnemy.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                    break;
                case EnemyRankType.Platinum:
                    sr.color = new Color(0.8f, 1.0f, 1.0f); // 빛나는 하늘색
                    spawnedEnemy.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                    break;
            }
        }
        Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.InitEnemy();
        }

        Debug.Log($"[소환됨] {randomRank.rankType} 등급의 {randomRace.raceType} {randomClass.classType} 출현!");
    }


}