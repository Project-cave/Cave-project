using UnityEngine;

public class EnemyWarriorClass : Enemy
{
    public override void AttackAction()
    {
        if (scanner.attackTarget != null)
        {
            StatHandler targetStat = scanner.attackTarget.GetComponent<StatHandler>();

            if (targetStat != null)
            {
                float finalDamage = stat.AttackPower * stat.DamageMultiplier;

                if (isCriticalContext)
                {
                    finalDamage *= stat.CriticalMultiplier;
                }
                targetStat.TakeDamage(Mathf.RoundToInt(finalDamage));
            }
        }
    }

    public override void OnCombatBehaviour()
    {
        rigid.linearVelocity = Vector2.zero;
    }
}