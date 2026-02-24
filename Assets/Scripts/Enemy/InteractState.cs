using UnityEngine;

public class InteractState : EnemyState
{
    Enemy owner;

    public InteractState(Enemy owner)
    {
        this.owner = owner;
    }

    #region 1. 상속

    public void Enter()
    {
        owner.anim.SetBool("RunBool", false);
    }

    public void Execute()
    {

        owner.rigid.linearVelocity = Vector2.zero;
    }

    public void Exit()
    {
        owner.rigid.linearVelocity = Vector2.zero;
    }

    #endregion


}