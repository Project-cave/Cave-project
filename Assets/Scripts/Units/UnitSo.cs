using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Object/unit")]
public class UnitSo : ScriptableObject
{
    public enum UnitRaceType { Goblin, Undead }
    public enum UnitRankType { Normal }
    public enum UnitAttackType { Melee, Ranged }

    [Header("유닛 데이터")]
    public string unitName;
    public int unitNum;
    public UnitRaceType raceType;
    public UnitRankType rankType;
    public int baseHP;
    public UnitAttackType attackType;
    public int baseAtk;
    public float baseAttackSpeed;
    public float baseAttackRange;
    public float damageMultiplier;
    public float collisionSpeed;
    public float baseMoveSpeed;
    public int baseDefence;
    public float critRate;
    public float critMultiplier;
    // 특수 기믹 안넣음

    [TextArea]
    public string unitDesc;

    [Header("해금 조건")]
    public int request;

    [TextArea]
    public string requestDesc;

    [Header("생산 조건")]
    public int material;

    [TextArea]
    public string materialDesc;

    public RuntimeAnimatorController animController;
    public GameObject DebuffPrefab;

}