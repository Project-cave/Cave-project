using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRangedClass : Enemy
{
    [Header("Bullet Settings")]
    public int bulletPoolIndex = 4;
    public int criticalPoolIndex = 3;

    public override void AttackAction()
    {
        if (scanner.attackTarget != null && GameManager.instance.pool != null)
        {
            Vector2 spawnPos = transform.position;

            GameObject bulletObj;
            if (isCriticalContext)
            {
                bulletObj = GameManager.instance.pool.Get(criticalPoolIndex);
            }
            else
            {
                bulletObj = GameManager.instance.pool.Get(bulletPoolIndex);
            }

            if (bulletObj == null) return;
            bulletObj.transform.position = spawnPos;

            Bullet bullet = bulletObj.GetComponent<Bullet>();

            if (bullet != null)
            {
                float finalDamage = stat.AttackPower * stat.DamageMultiplier;

                if (isCriticalContext)
                {
                    finalDamage *= stat.CriticalMultiplier;
                }

                Vector2 dir = ((Vector2)scanner.attackTarget.position - spawnPos).normalized;

                string dynamicTargetTag = scanner.attackTarget.tag;

                bullet.Init(gameObject.GetInstanceID(), finalDamage, 0, dir, stat.CollisionSpeed, dynamicTargetTag, transform);
            }
        }
    }

    public override void OnCombatBehaviour()
    {
        anim.SetBool("RunBool", false);

        Vector2 targetPos = scanner.attackTarget.position;
        Vector2 bestPos = GetBestShootingPos(targetPos);

        if (Vector2.Distance(transform.position, bestPos) <= 0.1f)
        {
            currentPath?.Clear();
            rigid.linearVelocity = Vector2.zero; // 제자리에 멈춰서 사격 대기
            return;
        }

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

        if (!HasPath) return;

        anim.SetBool("RunBool", true);
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

    public override void HandleHit(Transform attacker)
    {
        base.HandleHit(attacker);

        if (attacker == null) return;

        if (scanner.attackTarget != null && (Time.time - stat.LastAttackTime >= stat.CurrentAttackSpeed)) return;

        float distToAttacker = Vector2.Distance(transform.position, attacker.position);

        if (distToAttacker <= 1.5f || distToAttacker < scanner.attackRange * 0.5f)
        {
            scanner.aggroTarget = null;

            Vector2 fleePos = GetFleePosition(attacker.position);

            currentPath = pathFinder.getShortestPath(transform.position, fleePos);
            ChangeState(explore);
            isFleeing = true;
        }
    }

    public Vector2 GetFleePosition(Vector2 attackerPos)
    {
        Vector2 myPos = transform.position;
        Vector2 bestPos = myPos;
        float maxDistFromAttacker = Vector2.Distance(myPos, attackerPos);

        int range = 3;
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector2 checkPos = myPos + new Vector2(x, y);

                if (!scanner.IsTargetVisible(checkPos, myPos)) continue;

                float distFromAttacker = Vector2.Distance(checkPos, attackerPos);

                if (distFromAttacker > maxDistFromAttacker)
                {
                    maxDistFromAttacker = distFromAttacker;
                    bestPos = checkPos;
                }
                else if (distFromAttacker == maxDistFromAttacker)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        maxDistFromAttacker = distFromAttacker;
                        bestPos = checkPos;
                    }
                }
            }
        }

        return bestPos;
    }
}