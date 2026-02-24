using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public float attackRange;
    public LayerMask targetLayer;
    public LayerMask wallLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;
    public Transform attackTarget;
    PlayerMovement player;
    public bool inAttackRange;
    public HashSet<Vector3Int> Explored { get; protected set; } = new HashSet<Vector3Int>();

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
    }


    private void FixedUpdate()
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
        if(nearestTarget != null )
            attackTarget = GetAttackTarget();       
    }

    // 우선도가 가장 높은 것 중 가까운 것을 찾도록 수정
    Transform GetNearest()
    {
        Transform result = null;
        int bestPriority = int.MaxValue;
        float bestDist = float.MaxValue;

        Vector3 mypos = transform.position;

        foreach (RaycastHit2D target in targets)
        {
            if (target.transform == null || target.transform == transform) continue;

            Targetable targetInfo = target.transform.GetComponent<Targetable>();
            Vector3 targetPos = target.transform.position;

            if (targetInfo == null || !targetInfo.IsActive || !IsTargetVisible(targetPos)) continue;

            int curPriority = targetInfo.priority;
            float curDist = Vector3.Distance(mypos, targetPos);

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
        if (!nearestTarget.CompareTag("selectable") && !nearestTarget.CompareTag("Enemy")) return result;

        Vector3 mypos = transform.position;
        Vector3 targetPos = nearestTarget.position;
        float curDiff = Vector3.Distance(mypos,targetPos);

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
    public bool IsTargetVisible(Vector3 to)
    {
        Vector3 from = transform.position;

        RaycastHit2D hitWall = Physics2D.Linecast(from, to, wallLayer);

        return hitWall.collider == null;
    }

    // 타일 탐색
    public void ExploreTiles()
    {
        Vector3Int currentPos = Vector3Int.RoundToInt(transform.position);
        int scan = Mathf.RoundToInt(scanRange);

        for (int x = -scan; x <= scan; x++)
        {
            for (int y = -scan; y <= scan; y++)
            {
                Vector3Int pos = currentPos + new Vector3Int(x, y, 0);

                if (Vector3.Distance(currentPos, pos) >= scan || Explored.Contains(pos) || !IsTargetVisible(pos)) continue;

                Explored.Add(pos);
            }
        }
    }
}
