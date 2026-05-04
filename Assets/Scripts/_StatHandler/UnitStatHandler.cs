using UnityEngine;

public class UnitStatHandler : StatHandler
{
    public void InitializeStats(UnitSo unitData)
    {
        MaxHP = unitData.combatStats.baseHP;
        AttackPower = unitData.combatStats.baseAtk;
        BaseAttackSpeed = unitData.combatStats.baseAttackSpeed;
        DamageMultiplier = unitData.combatStats.damageMultiplier;
        CollisionSpeed = unitData.combatStats.collisionSpeed;
        CriticalRate = Mathf.RoundToInt(unitData.combatStats.critRate * 100);
        CriticalMultiplier = unitData.combatStats.critMultiplier;

        /* if (unitData.attackType == UnitSo.UnitAttackType.Ranged)
        {
            if (unitData.bulletData != null) projectileRadius = unitData.bulletData.radius;
            else projectileRadius = 0.2f;
        }
        else projectileRadius = 0f; */

        CurrentHP = MaxHP;
        LastAttackTime = -AttackMotionDelay;
        isDead = false;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        if (rigid != null)
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}