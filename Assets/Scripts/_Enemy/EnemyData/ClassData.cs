using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RaceInfo
{
    public EnemyRaceType race;
    public RuntimeAnimatorController animController;
    public float collisionMultiplier;
}

[CreateAssetMenu(fileName = "New Class Data", menuName = "EnemyStats/Class Data")]
public class ClassData : ScriptableObject
{
    [Header("기본 정보")]
    public EnemyClassType classType;
    public EnemyAttackType attackType;

    [Header("직업 스탯 배율")]
    public float hpMultiplier;
    public float atkMultiplier;
    public float defMultiplier;
    public float moveSpeedMultiplier;

    [Header("전투 능력치")]
    public float baseAttackSpeed;
    public float baseAttackRange;

    [Header("콜리전 및 특수 기믹")]
    public BullletSo bulletData;
    public float collisionSpeed;
    public string collisionSpecial; // TODO: 상태이상 enum 만들기
    public string specialGimmick;   // TODO: 현재 없음

    [Header("종족 정보")]
    public List<RaceInfo> raceInfo;

    [Header("임시 정보")]
    public Sprite classShape;
}