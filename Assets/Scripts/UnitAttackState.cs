using UnityEngine;

public class UnitAttackState : IState
{
    private Unit unit;

    public UnitAttackState(Unit unit)
    {
        this.unit = unit;
    }

    public void Enter()
    {
        unit.Animator.PlayAttack();
        unit.Movement.ClearPath();
    }

    public void Update()
    {
        if (unit.HasPlayerCommand && unit.Movement.HasPath)
        {
            unit.ChangeState(unit.MoveState);
            return;
        }

        // 공격
        if (unit.Combat.CanAttack())
        {
            unit.Combat.AimAtTarget();
        }

        // 추격
        else if (unit.Scanner.nearestTarget != null && !unit.Combat.IsEnemyInRange())
        {
            unit.Movement.SetDestination(unit.Scanner.nearestTarget.position);
            unit.ChangeState(unit.MoveState);
        }

        else
        {
            unit.ChangeState(unit.IdleState);
        }
    }

    public void Exit()
    {
        unit.rigid.linearVelocity = Vector2.zero;
    }
}