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
        owner.currentPath =
            owner.pathFinder.FindNearestUnexplored(owner.transform.position, owner.scanner.Explored);

        if (owner.HasPath)
        {
            owner.rigid.linearVelocity = Vector2.zero;
        }
    }

    #endregion
}