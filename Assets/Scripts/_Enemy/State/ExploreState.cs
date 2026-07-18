using System.Collections.Generic;
using UnityEngine;

public class ExploreState : EnemyState
{
    Enemy owner;

    public ExploreState(Enemy owner)
    {
        this.owner = owner;
    }

    #region 1. 상속

    public void Enter()
    {
        if (owner.anim != null && owner.anim.runtimeAnimatorController != null)
        {
            owner.anim.SetBool("RunBool", true);
        }

        owner.lastDetectedTarget = null;
    }

    public void Execute()
    {
        if (owner.IsTargetActive() && (!owner.isFleeing || (owner.scanner.attackTarget != null &&
            (Time.time - owner.stat.LastAttackTime >= owner.stat.CurrentAttackSpeed))))
        {
            owner.ChangeState(owner.chase);
            return;
        }

        if (!owner.HasPath && owner.isFleeing)
        {
            owner.isFleeing = false;
            return;
        }

        if (!owner.HasPath)
        {
            FindNextDestination();
        }

        if (!owner.HasPath)
        {
            owner.rigid.linearVelocity = Vector2.zero;
            return;
        }

        owner.MoveToDestination();
    }

    public void Exit()
    {
        owner.rigid.linearVelocity = Vector2.zero;
        owner.currentPath = null;
    }

    #endregion

    #region 2. 함수

    public void FindNextDestination()
    {
        Vector2 snappedStart = GridConverter.SnapToLogicalGridCenter(owner.transform.position);

        Vector2 currentDir = owner.rigid.linearVelocity.normalized;
        if (currentDir == Vector2.zero)
        {
            if (owner.scanner.nearestTarget != null)
                currentDir = (owner.scanner.nearestTarget.position - owner.transform.position).normalized;
            else
            {
                Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
                currentDir = directions[Random.Range(0, directions.Length)];
            }
        }

        int turnCost = owner.stat.TurnPenalty;
        int uTurnCost = owner.stat.UTurnPenalty;

        if (Random.Range(0, 100) < owner.stat.CuriosityRate) turnCost = 10; uTurnCost = 10;

        PathWeight weight = new PathWeight(currentDir, turnCost, uTurnCost);

        LinkedList<Vector2> rawPath =
            owner.pathFinder.FindNearestUnexplored(snappedStart, owner.scanner.Explored, weight);

        owner.currentPath = GridConverter.CompressPathToLogicalGrid(rawPath);

        if (owner.HasPath)
        {
            owner.rigid.linearVelocity = Vector2.zero;
        }
    }

    #endregion
}