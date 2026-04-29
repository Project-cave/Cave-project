using UnityEngine;

public class AttackState : EnemyState
{
    Enemy owner;

    public AttackState(Enemy owner)
    {
        this.owner = owner;
    }

    #region 1. 상속

    public void Enter()
    {
        if (owner.anim != null && owner.anim.runtimeAnimatorController != null)
        {
            owner.anim.SetBool("RunBool", false);
        }
        owner.rigid.linearVelocity = Vector2.zero;
    }

    public void Execute()
    {
        if (!owner.IsTargetActive())
        {
            owner.ChangeState(owner.explore);
            return;
        }

        if (owner.scanner.attackTarget == null)
        {
            owner.ChangeState(owner.chase);
            return;
        }

        float dist = Vector2.Distance(owner.transform.position, owner.scanner.attackTarget.position);
        bool isWallBlocked = !owner.scanner.CanAttack(owner.scanner.attackTarget.position,
            owner.transform.position, owner.stat.projectileRadius);

        if (dist > owner.scanner.attackRange || isWallBlocked)
        {
            owner.ChangeState(owner.chase);
            return;
        }

        owner.LookAt(owner.scanner.attackTarget.position);

        float cooldown = owner.stat.CurrentAttackSpeed;
        float lastAttack = owner.stat.LastAttackTime;

        if (Time.time - lastAttack >= cooldown)
        {
            PerformAttack();
        }
        else
        {
            owner.OnCombatBehaviour();
        }
    }

    public void Exit()
    {
        owner.rigid.linearVelocity = Vector2.zero;
    }

    #endregion


    #region 2. 함수

    void PerformAttack()
    {
        owner.rigid.linearVelocity = Vector2.zero;

        if (owner.anim != null && owner.anim.runtimeAnimatorController != null)
        {
            owner.anim.SetBool("RunBool", false);
        }

        bool isCrit = (Random.Range(0, 100) <= owner.stat.CriticalRate);
        owner.isCriticalContext = isCrit;

        int skillIndex = isCrit ? 1 : 0;

        owner.anim.SetInteger("AttackIndex", skillIndex);
        owner.anim.SetTrigger("AttackTrigger");

        owner.stat.OnAttack();
    }

    #endregion
}