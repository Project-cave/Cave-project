using UnityEditor;
using UnityEngine;

public class UnitCSVImporter : MultiCSVImporterBase
{
    [MenuItem("Tools/CSV Importer/Unit Importer")]
    public static void OpenWindow() => GetWindow<UnitCSVImporter>("Unit CSV Importer");

    protected override string WindowTitle => "유닛 CSV → SO 변환기 (다중 시트)";
    protected override string outputFolderPath => "Assets/Resources/Data/SO/Units";
    protected override string KeyColumnName => "unitName";

    protected override string[] CSVFilePaths => new string[]
    {
        "Assets/Resources/Data/CSV/units_info.csv",
        "Assets/Resources/Data/CSV/units_combat.csv",
        "Assets/Resources/Data/CSV/units_unlock.csv",
    };

    protected override void CreateSOFromMergedRow(string key, MergedRow row)
    {
        // 어느 시트에 어떤 컬럼이 있는지 명확하게 보임
        SaveOrUpdateAsset<UnitSo>(key, so =>
        {
            so.info = new UnitInfo
            {
                unitName = key,
                unitNum = ParseInt(row.Get("units_info", "unitNum")),
                raceType = ParseEnum(row.Get("units_info", "raceType"), UnitSo.UnitRaceType.Variant),
                rankType = ParseInt(row.Get("units_info", "rankType")),
                attackType = ParseEnum(row.Get("units_info", "attackType"), UnitSo.UnitAttackType.Normal_Melee),
                unitDesc = row.Get("units_info", "unitDesc"),
            };

            so.combatStats = new UnitCombatStats
            {
                baseHP = ParseInt(row.Get("units_combat", "baseHP")),
                baseAtk = ParseInt(row.Get("units_combat", "baseAtk")),
                baseDefence = ParseInt(row.Get("units_combat", "baseDefence")),
                baseAttackSpeed = ParseFloat(row.Get("units_combat", "baseAtkSpeed")),
                baseAttackRange = ParseFloat(row.Get("units_combat", "baseAtkRange")),
                baseMoveSpeed = ParseFloat(row.Get("units_combat", "baseMoveSpeed")),
                damageMultiplier = ParseFloat(row.Get("units_combat", "damageMultiplier")),
                collisionSpeed = ParseFloat(row.Get("units_combat", "collisionSpeed")),
                critRate = ParseFloat(row.Get("units_combat", "critRate")),
                critMultiplier = ParseFloat(row.Get("units_combat", "critMultiplier")),
            };

            so.unlockData = new UnitUnlockData
            {
                request = ParseInt(row.Get("units_unlock", "request")),
                requestDesc = row.Get("units_unlock", "requestDesc"),
            };

            so.productionData = new UnitProductionData
            {
                material = ParseInt(row.Get("units_unlock", "material")),
                materialDesc = row.Get("units_unlock", "materialDesc"),
            };
        });
    }
}