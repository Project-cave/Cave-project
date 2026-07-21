using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    private List<GameObject> activeEnemies = new List<GameObject>();

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

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy != null && !activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        if (StageManager.instance == null) return;

        if (StageManager.instance.isWaveActive && activeEnemies.Count == 0)
        {
            StageManager.instance.isWaveActive = false;
            StageManager.instance.OnWaveCleared(StageManager.instance.isCurrentWaveBoss);
        }
    }
}