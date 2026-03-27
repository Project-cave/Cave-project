using UnityEngine;

[CreateAssetMenu(fileName = "New Race Data", menuName = "EnemyStats/Race Data")]
public class RaceData : ScriptableObject
{
    [Header("기본 정보")]
    public EnemyRaceType raceType;

    [Header("종족 기본 스탯")]
    public int baseHP;
    public int baseAtk;
    public int baseDef;
    public float baseMoveSpeed;
    public int baseSanity;

    [Header("콜리전 및 특수 기믹")]
    public string specialGimmick;   // TODO: 현재 없음
}