using UnityEngine;

[CreateAssetMenu(fileName = "New Rank Data", menuName = "EnemyStats/Rank Data")]
public class RankData : ScriptableObject
{
    [Header("기본 정보")]
    public EnemyRankType rankType;

    [Header("스탯 배율")]
    public float hpMultiplier;
    public float atkMultiplier;
    public float defMultiplier;
    public float moveSpeedMultiplier;
    public float sanMultiplier;
    public float attackSpeedMultiplier;

    [Header("전투 능력치")]
    public float meleeRangeMultiplier;
    public float rangedRangeMultiplier;

    [Header("치명타")]
    public float critChance;
    public float critMultiplier;

    [Header("콜리전 및 특수 기믹")]
    public string specialGimmick;   // TODO: 현재 없음
}