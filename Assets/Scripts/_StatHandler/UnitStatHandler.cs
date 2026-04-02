using UnityEngine;

public class UnitStatHandler : StatHandler
{
    public void InitializeStats(UnitSo unitData)
    {
        MaxHP = unitData.baseHP;
        AttackPower = unitData.baseAtk;
        BaseAttackSpeed = unitData.baseAttackSpeed;
        AttackRange = unitData.baseAttackRange;
        DamageMultiplier = unitData.damageMultiplier;
        CollisionSpeed = unitData.collisionSpeed;
        MoveSpeed = unitData.baseMoveSpeed;
        CriticalRate = Mathf.RoundToInt(unitData.critRate * 100);
        CriticalMultiplier = unitData.critMultiplier;

        CurrentHP = MaxHP;
        LastAttackTime = -AttackMotionDelay;
        isDead = false;
    }
}