using UnityEngine;

public class UnitStatHandler : StatHandler
{
    public void InitializeStats(UnitSo unitData)
    {
        MaxHP = unitData.baseHP;
        AttackPower = unitData.baseAtk;
        BaseAttackSpeed = unitData.baseAttackSpeed;
        DamageMultiplier = unitData.damageMultiplier;
        CollisionSpeed = unitData.collisionSpeed;
        CriticalRate = Mathf.RoundToInt(unitData.critRate * 100);
        CriticalMultiplier = unitData.critMultiplier;

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