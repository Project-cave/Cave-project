using UnityEngine;

public class IdleState : IState
{
    private Unit unit;

    public IdleState(Unit unit)
    {
        this.unit = unit;
    }

    public void Enter()
    {
        unit.Animator.PlayIdle();
        unit.ClearPlayerCommand();
    }

    public void Update()
    {
        if (unit.HasPlayerCommand) return;

        if (unit.Scanner.nearestTarget != null)
        {
            // === 범위 안 (공격) ===
            if (unit.Combat.IsEnemyInRange())
            {
                unit.ChangeState(unit.AttackState);
            }

            // === 범위 밖 (추격) ===
            else
            {
                unit.Movement.SetDestination(unit.Scanner.nearestTarget.position);
                unit.ChangeState(unit.MoveState);
            }
        }
    }

    public void Exit()
    {
        unit.rigid.linearVelocity = Vector2.zero;
    }
}
