using UnityEngine;

public class EnemyStatHandler : StatHandler
{
    [field: Header("Stat")]
    [field: SerializeField] public int Sanity { get; protected set; }

    [field:Header("PathFinding")]
    [field: SerializeField] public int TurnPenalty { get; protected set; }
    [field: SerializeField] public int UTurnPenalty { get; protected set; }
    [Range(0, 100)]
    [field: SerializeField] public int CuriosityRate { get; protected set; }

    [Header("Data")]
    public RaceData raceData;
    public ClassData classData;
    public RankData rankData;

    public void InitializeStats()
    {
        MaxHP = Mathf.RoundToInt(raceData.baseHP * (classData.hpMultiplier + rankData.hpMultiplier - 1));
        AttackPower = Mathf.RoundToInt(raceData.baseAtk * (classData.atkMultiplier + rankData.atkMultiplier - 1));
        Defence = Mathf.RoundToInt(raceData.baseDef * (classData.defMultiplier + rankData.defMultiplier - 1));
        BaseMoveSpeed = raceData.baseMoveSpeed * (classData.moveSpeedMultiplier + rankData.moveSpeedMultiplier - 1);
        Sanity = Mathf.RoundToInt(raceData.baseSanity * rankData.sanMultiplier);
        BaseAttackSpeed = classData.baseAttackSpeed * rankData.attackSpeedMultiplier;
        if (classData.attackType == EnemyAttackType.Melee) AttackRange = classData.baseAttackRange * rankData.meleeRangeMultiplier;
        else AttackRange = classData.baseAttackRange * rankData.rangedRangeMultiplier;
        DamageMultiplier = 1.0f;
        if (classData.attackType == EnemyAttackType.Ranged)
        {
            if (classData.bulletData != null) projectileRadius = classData.bulletData.radius;
            else projectileRadius = 0.2f;
        }
        else projectileRadius = 0f;

        if (classData.raceInfo != null)
        {
            foreach (var info in classData.raceInfo)
            {
                if ((info.race == raceData.raceType))
                {
                    DamageMultiplier = info.collisionMultiplier;
                    break;
                }
            }
        }

        CollisionSpeed = classData.collisionSpeed;
        CriticalRate = Mathf.RoundToInt(rankData.critChance * 100);
        CriticalMultiplier = rankData.critMultiplier;

        // 추후 난이도에 따라 값이 바뀌도록 수정 예정
        TurnPenalty = 20;
        UTurnPenalty = 50;
        CuriosityRate = 10;

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