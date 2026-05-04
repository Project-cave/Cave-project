using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Object/Unit")]
public class UnitSo : ScriptableObject
{
    public enum UnitRaceType { Variant, Beast, Undead, Flight, Devil, Dragon }
    public enum UnitAttackType { Normal_Melee, Speed_Melee, Power_Melee, Range_Normal, Range_Magic }

    [Header("기본 정보")]
    public UnitInfo info;

    [Header("전투 스탯")]
    public UnitCombatStats combatStats;

    [Header("해금 조건")]
    public UnitUnlockData unlockData;

    [Header("생산 조건")]
    public UnitProductionData productionData;

    [Header("리소스 (인스펙터에서 직접 등록)")]
    public GameObject unitPrefab;
    public RuntimeAnimatorController animController;
    public GameObject debuffPrefab;
}

[System.Serializable]
public class UnitInfo
{
    public string unitName;
    public int unitNum;
    public UnitSo.UnitRaceType raceType;
    public int rankType;
    public UnitSo.UnitAttackType attackType;
    [TextArea] public string unitDesc;
}

[System.Serializable]
public struct UnitCombatStats
{
    public int baseHP;
    public int baseAtk;
    public int baseDefence;
    public float baseAttackSpeed;
    public float baseAttackRange;
    public float baseMoveSpeed;
    public float damageMultiplier;
    public float collisionSpeed;
    public float critRate;
    public float critMultiplier;
}

[System.Serializable]
public struct UnitUnlockData
{
    public int request;
    [TextArea] public string requestDesc;
}

[System.Serializable]
public struct UnitProductionData
{
    public int material;
    [TextArea] public string materialDesc;
}