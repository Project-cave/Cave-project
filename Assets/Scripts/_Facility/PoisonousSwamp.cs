using UnityEngine;
using System.Collections.Generic;

public class PoisonousSwamp : MonoBehaviour
{
    [SerializeField] private int trapDamage = 1;
    [SerializeField] private float reductionSpeed = -0.5f;
    Dictionary<EnemyStatHandler, float> enemiesDebuff = new Dictionary<EnemyStatHandler, float>();
    List<EnemyStatHandler> deadEnemies = new List<EnemyStatHandler>();

    private void Update()
    {
        if (enemiesDebuff.Count == 0) return;

        deadEnemies.Clear();

        foreach (KeyValuePair<EnemyStatHandler, float> kvp in enemiesDebuff)
        {
            EnemyStatHandler enemy = kvp.Key;
            float time = kvp.Value;

            if (enemy == null || enemy.isDead)
            {
                deadEnemies.Add(enemy);
                continue;
            }

            if (Time.time - time < 1.0f) continue;

            enemiesDebuff[enemy] = Time.time;
            enemy.TakeDamage(trapDamage, null);
        }

        for (int i = 0; i < deadEnemies.Count; i++)
        {
            enemiesDebuff.Remove(deadEnemies[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStatHandler enemy = other.GetComponent<EnemyStatHandler>();

        if (enemy != null && !enemy.isDead && !enemiesDebuff.ContainsKey(enemy))
        {
            enemiesDebuff.Add(enemy, Time.time - 1.0f);
            enemy.AddMoveSpeedPercentage(reductionSpeed);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        EnemyStatHandler enemy = other.GetComponent<EnemyStatHandler>();

        if (enemy != null && enemiesDebuff.ContainsKey(enemy))
        {
            enemiesDebuff.Remove(enemy);
            enemy.AddMoveSpeedPercentage(-reductionSpeed);
        }
    }
}
