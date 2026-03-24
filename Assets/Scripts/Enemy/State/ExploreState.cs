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
        owner.anim.SetBool("RunBool", true);
    }

    public void Execute()
    {
        if (owner.scanner.nearestTarget != null)
        {
            owner.ChangeState(owner.chase);
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