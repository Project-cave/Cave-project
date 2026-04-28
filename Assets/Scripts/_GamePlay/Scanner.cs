using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public float attackRange;
    public LayerMask targetLayer;
    public LayerMask wallLayer;
    private Collider2D[] targets = new Collider2D[30];
    private int hitCount = 0;
    private ContactFilter2D targetFilter;
    public Transform nearestTarget;
    public Transform attackTarget;
    [HideInInspector] public Transform aggroTarget;
    [HideInInspector] public float lastAggroTime;
    public float aggroTime = 11.0f;
    EnemyStatHandler statHandler;
    public bool inAttackRange;
    public HashSet<Vector3Int> Explored { get; protected set; } = new HashSet<Vector3Int>();

    private void Awake()
    {
        statHandler = GetComponent<EnemyStatHandler>();

        targetFilter = new ContactFilter2D();
        targetFilter.useLayerMask = true;
        targetFilter.SetLayerMask(targetLayer);
        targetFilter.useTriggers = Physics2D.queriesHitTriggers;
    }

    private void FixedUpdate()
    {
        hitCount = Physics2D.OverlapCircle(transform.position, scanRange, targetFilter, targets);

        if (aggroTarget != null)
        {
            if (!aggroTarget.gameObject.activeSelf) aggroTarget = null;
            else
            {
                float dist = Vector2.Distance(transform.position, aggroTarget.position);
                bool inSight = dist <= scanRange && IsTargetVisible(aggroTarget.position, transform.position);

                if (!inSight && Time.time - lastAggroTime > aggroTime) aggroTarget = null;
            }
        }

        nearestTarget = GetNearest();

        if (aggroTarget != null)
        {
            if (nearestTarget == null)
            {
                nearestTarget = aggroTarget;
            }
            else
            {
                int aggroPriority = aggroTarget.GetComponent<Targetable>().priority;
                int nearestPriority = nearestTarget.GetComponent<Targetable>().priority;

                if (nearestPriority > aggroPriority)
                {
                    nearestTarget = aggroTarget;
                }
                else
                {
                    aggroTarget = null;
                }
            }
        }

        if(nearestTarget == null)
        {
            attackTarget = null;
            return;
        }
        attackTarget = GetAttackTarget();
    }

    // 우선도가 가장 높은 것 중 가까운 것을 찾도록 수정
    Transform GetNearest()
    {
        Transform result = null;
        int bestPriority = int.MaxValue;
        float bestDist = float.MaxValue;

        Vector3 mypos = transform.position;

        bool isAttackingBuilding = false;
        int currentBuildingPriority = int.MaxValue;

        if (attackTarget != null)
        {
            Targetable currentTargetInfo = attackTarget.GetComponent<Targetable>();
            if (currentTargetInfo != null && currentTargetInfo.priority > 1)
            {
                isAttackingBuilding = true;
                currentBuildingPriority = currentTargetInfo.priority;
            }
        }

        for (int i = 0; i < hitCount; i++)
        {
            Transform target = targets[i].transform;

            if (target == null || target == transform) continue;

            Targetable targetInfo = target.transform.GetComponent<Targetable>();

            if (targetInfo == null || !targetInfo.IsActive || !IsTargetVisible(target.position, transform.position)) continue;

            if (statHandler != null && statHandler.rankData != null)
            {
                if ((int)statHandler.rankData.rankType < (int)targetInfo.requiredRank) continue;
            }

            int curPriority = targetInfo.priority;
            float curDist = Vector3.Distance(mypos, target.position);


            if (isAttackingBuilding && curPriority < currentBuildingPriority && curDist > attackRange)
            {
                continue;
            }

            if (curPriority < bestPriority)
            {
                bestPriority = curPriority;
                bestDist = curDist;
                result = target.transform;
            }
            else if (curPriority == bestPriority && curDist < bestDist)
            {
                bestDist = curDist;
                result = target.transform;
            }
        }
        return result;
    }

    Transform GetAttackTarget()
    {
        Transform result = null;

        // 몬스터와 유닛만 타겟 설정 가능하게 수정
        if (!nearestTarget.CompareTag("selectable") && !nearestTarget.CompareTag("Enemy") &&
            !nearestTarget.CompareTag("Facility")) return result;

        Vector3 mypos = transform.position;
        Vector3 targetPos = nearestTarget.position;
        float curDiff = Vector3.Distance(mypos, targetPos);

        if (curDiff < attackRange)
        {
            inAttackRange = true;
            result = nearestTarget;
        }
        else
        {
            inAttackRange = false;
        }

        return result;
    }

    // 라인캐스팅
    public bool IsTargetVisible(Vector3 to, Vector3 from)
    {
        RaycastHit2D hitWall = Physics2D.Linecast(from, to, wallLayer);

        return hitWall.collider == null;
    }

    public bool CanAttack(Vector3 to, Vector3 from, float radius)
    {
        Vector2 dir = (to - from).normalized;
        float dist = Vector2.Distance(from, to);

        RaycastHit2D hitWall = Physics2D.CircleCast(from, radius, dir, dist, wallLayer);

        return hitWall.collider == null;
    }

    // 타일 탐색
    public void ExploreTiles()
    {
        int currentX = Mathf.FloorToInt(transform.position.x);
        int currentY = Mathf.FloorToInt(transform.position.y);
        Vector3 currentPos = new Vector3(currentX + 0.5f, currentY + 0.5f, 0);

        int scan = Mathf.CeilToInt(scanRange);

        for (int x = -scan; x <= scan; x++)
        {
            for (int y = -scan; y <= scan; y++)
            {
                Vector3Int targetIndex = new Vector3Int(currentX + x, currentY + y, 0);
                Vector3 targetPos = new Vector3(currentX + x + 0.5f, currentY + y + 0.5f, 0);

                if (Vector3.Distance(currentPos, targetPos) > scanRange || Explored.Contains(targetIndex) ||
                    !IsTargetVisible(targetPos, currentPos)) continue;

                Explored.Add(targetIndex);
            }
        }
    }
}