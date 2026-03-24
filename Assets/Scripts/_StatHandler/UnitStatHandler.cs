using UnityEngine;

public class UnitStatHandler : StatHandler
{
    [Header("Data")]
    [SerializeField] public UnitSo unitData;

    public void InitializeStats()
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