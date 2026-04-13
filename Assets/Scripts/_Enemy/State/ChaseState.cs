using UnityEngine;

public class ChaseState : EnemyState
{
    Enemy owner;

    public ChaseState(Enemy owner)
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

        if (owner.scanner.nearestTarget != null && owner.scanner.nearestTarget != owner.lastDetectedTarget)
        {
            owner.ShowEmoticon("Detection");
            owner.lastDetectedTarget = owner.scanner.nearestTarget;
        }

        UpdatePathToTarget();
    }

    public void Execute()
    {
        if (!owner.IsTargetActive())
        {
            owner.ChangeState(owner.explore);
            return;
        }

        if (owner.scanner.attackTarget != null)
        {
            if (owner.scanner.IsTargetVisible(owner.scanner.nearestTarget.position, owner.transform.position))
            {
                owner.ChangeState(owner.attack);
                return;
            }
        }

        if (Vector2.Distance(owner.transform.position, owner.scanner.nearestTarget.position) <= 0.1f)
        {
            owner.rigid.linearVelocity = Vector2.zero;
        }
        else if (!owner.HasPath)
        {
            UpdatePathToTarget();
            if (!owner.HasPath) return;
        }


        if (owner.HasPath)
        {
            Vector2Int goal = Vector2Int.RoundToInt(owner.currentPath.Last.Value);

            if (goal != Vector2Int.RoundToInt(owner.scanner.nearestTarget.transform.position))
            {
                UpdatePathToTarget();
                if (!owner.HasPath) return;
            }

            owner.MoveToDestination();
        }
    }

    public void Exit()
    {
        owner.rigid.linearVelocity = Vector2.zero;
        owner.currentPath = null;
    }

    #endregion

    #region 2. 함수

    private void UpdatePathToTarget()
    {
        if (owner.scanner.nearestTarget == null) return;

        owner.currentPath =
            owner.pathFinder.getShortestPath(owner.transform.position, owner.scanner.nearestTarget.position);
    }

    #endregion
}