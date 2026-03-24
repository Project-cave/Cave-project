using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRangedClass : Enemy
{
    [Header("Bullet Settings")]
    public GameObject projectilePrefab;

    public override void AttackAction()
    {
        if (scanner.attackTarget != null && projectilePrefab != null)
        {
            Vector2 spawnPos = transform.position;
            GameObject bulletObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            Bullet bullet = bulletObj.GetComponent<Bullet>();

            if (bullet != null)
            {
                float finalDamage = stat.AttackPower * stat.DamageMultiplier;

                if (isCriticalContext)
                {
                    finalDamage *= stat.CriticalMultiplier;
                }

                // bullet.Init(scanner.attackTarget, Mathf.RoundToInt(finalDamage));
            }
        }
    }

    public override void OnCombatBehaviour()
    {
        Vector2 targetPos = scanner.attackTarget.position;
        Vector2 bestPos = GetBestShootingPos(targetPos);
        
        if (!HasPath)
        {
            currentPath = pathFinder.getShortestPath(transform.position, bestPos);
            if (!HasPath)
            {
                pathFinder.getShortestPath(transform.position, targetPos);
            }
        }

        if (!HasPath) return;

        Vector2 goal = currentPath.Last.Value;
        bool targetMoved = Vector2.Distance(bestPos, goal) > 1.5f;
        bool canAttack = scanner.IsTargetVisible(goal, targetPos) &&
            (Vector2.Distance(goal, targetPos) <= scanner.attackRange);
        
        if (targetMoved || !canAttack)
        {
            currentPath = pathFinder.getShortestPath(transform.position, bestPos);
            if (!HasPath)
            {
                pathFinder.getShortestPath(transform.position, targetPos);
            }
        }

        MoveToDestination();
    }

    // 함수
    private Vector2 GetBestShootingPos(Vector2 targetPos)
    {
        int range = Mathf.CeilToInt(scanner.attackRange);
        Vector2 bestPos = targetPos;

        float maxDist = -1f;
        float minMove = int.MaxValue;

        int xGreater = transform.position.x.CompareTo(targetPos.x);
        int yGreater = transform.position.y.CompareTo(targetPos.y);

        float invSqrtFive = 1 / Mathf.Sqrt(5);

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector2 checkPos = targetPos + new Vector2(x, y);

                float dist = Vector2.Distance(checkPos, targetPos);
                float moveDist = Vector2.Distance(transform.position, checkPos);

                if (dist > scanner.attackRange || !scanner.IsTargetVisible(checkPos, targetPos) ||
                    !scanner.IsTargetVisible(checkPos, transform.position)) continue;

                float targetDist = Vector2.Distance(transform.position, targetPos);

                if ((x * xGreater < 0 && y * yGreater < 0) || ((x * xGreater < 0 || y * yGreater < 0) &&
                    targetDist <= 2 * moveDist * invSqrtFive)) continue;

                if (dist > maxDist)
                {
                    maxDist = dist;
                    minMove = moveDist;
                    bestPos = checkPos;
                }
                else if (dist == maxDist)
                {
                    if (moveDist < minMove)
                    {
                        minMove = moveDist;
                        bestPos = checkPos;
                    }
                }
            }
        }

        return bestPos;
    }
}