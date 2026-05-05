using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Object/Unit")]
public class UnitSo : ScriptableObject
{
    public enum UnitRaceType { Variant, Beast, Undead, Flight, Devil, Dragon }
    public enum UnitAttackType { Normal_Melee, Speed_Melee, Power_Melee, Range_Normal, Range_Magic }

    [Header("Basic Info")]
    public UnitInfo info;

    [Header("Combat Stats")]
    public UnitCombatStats combatStats;

    [Header("Unlock Data")]
    public UnitUnlockData unlockData;

    [Header("Production Data")]
    public UnitProductionData productionData;

    [Header("Resources")]
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
public class UnitUnlockData
{
    public int request;
    [TextArea] public string requestDesc;
    public List<UnitSo> nextUnits;
    public Vector2 nodePosition;
}

[System.Serializable]
public struct UnitProductionData
{
    public int material;
    [TextArea] public string materialDesc;
}