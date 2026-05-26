using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy != null && !activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public void UnRegisterEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    // 씬 나가기 직전에 호출
    public void SaveSnapshot()
    {
        if (MapManager.instance == null) return;

        Dictionary<int, string> enemyData = new Dictionary<int, string>();

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            GameObject enemy = activeEnemies[i];
            if (enemy == null || !enemy.activeSelf) continue;

            EnemyStatHandler stat = enemy.GetComponent<EnemyStatHandler>();
            if (stat == null || stat.raceData == null || stat.rankData == null || stat.classData == null) continue;

            string value = $"{stat.raceData.name}|{stat.rankData.name}|{stat.classData.name}|" +
                           $"{enemy.transform.position.x}|{enemy.transform.position.y}";

            enemyData[i] = value;
        }

        MapManager.instance.UpdateCurrentEnemies(enemyData);
    }
}
