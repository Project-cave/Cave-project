using UnityEngine;
using System.Collections.Generic;

public class TransferTrap : MonoBehaviour
{
    private static List<TransferTrap> allTraps = new List<TransferTrap>();

    private static Dictionary<Collider2D, float> cooldowns = new Dictionary<Collider2D, float>();

    private bool trigger = true;

    private void OnEnable()
    {
        if (!allTraps.Contains(this))
            allTraps.Add(this);
    }

    private void OnDisable()
    {
        if (allTraps.Contains(this))
            allTraps.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!trigger) return;

        EnemyStatHandler stat = other.GetComponent<EnemyStatHandler>();
        if (stat == null || stat.isDead) return;

        if (cooldowns.TryGetValue(other, out float lastTime))
        {
            if (Time.time - lastTime < 1.0f) return;
        }

        ExecuteTeleport(other);
    }

    private void ExecuteTeleport(Collider2D col)
    {
        if (allTraps.Count <= 1) return;

        List<TransferTrap> candidates = new List<TransferTrap>();

        foreach (TransferTrap trap in allTraps)
        {
            if (trap != this)
            {
                candidates.Add(trap);
            }
        }

        int count = candidates.Count;

        if (count > 0)
        {
            int randomIndex = Random.Range(0, count);
            TransferTrap destination = candidates[randomIndex];

            trigger = false;

            cooldowns[col] = Time.time;

            col.transform.position = destination.transform.position;

            Rigidbody2D rigid = col.GetComponent<Rigidbody2D>();
            if (rigid != null)
            {
                rigid.linearVelocity = Vector2.zero;
            }

            ResetState(col.gameObject);
        }
    }

    private void ResetState(GameObject obj)
    {
        Enemy enemy = obj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.currentPath?.Clear();
            if (enemy.scanner != null)
            {
                enemy.scanner.aggroTarget = null;
                enemy.scanner.nearestTarget = null;
                enemy.scanner.attackTarget = null;
            }
            enemy.ChangeState(enemy.explore);
            return;
        }
    }
}
