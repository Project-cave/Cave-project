using UnityEngine;

public class MoveState : IState
{
    private Unit unit;

    public MoveState(Unit unit)
    {
        this.unit = unit;
    }

    public void Enter()
    {
        unit.Animator.PlayMove();
    }

    public void Update()
    {
        // === 공격 범위 안 && 명령 x (자동 공격) ===
        if (unit.Combat.IsEnemyInRange() && !unit.hasPlayerCommand)
        {
            unit.ChangeState(unit.AttackState);
            return;
        }

        // === 적 추격 경로 업데이트 ===
        if (!unit.HasPlayerCommand && unit.Scanner.nearestTarget != null)
        {
            float distanceToTarget = Vector2.Distance(
                unit.transform.position,
                unit.Scanner.nearestTarget.position
            );

            // === 경로 없음 && 너무 멈 (재설정) ===
            if (!unit.Movement.HasPath || distanceToTarget > 1f)
            {
                unit.Movement.SetDestination(unit.Scanner.nearestTarget.position);
            }
        }


        if (unit.Movement.HasPath)
        {
            unit.Movement.Move();
            Vector2 direction = unit.Movement.NextWaypoint - (Vector2)unit.transform.position;
            unit.Animator.FaceDirection(direction);
        }
        else
        {
            if (unit.Combat.IsEnemyInRange())
            {
                unit.ChangeState(unit.AttackState);
            }
            else
            {
                unit.ChangeState(unit.IdleState);
            }
        }
    }

    public void Exit()
    {
    }
}