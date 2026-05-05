using UnityEditor;

public class SkillCSVImporter : CSVImporterBase
{
    [MenuItem("Tools/CSV Importer/Skill Importer")]
    public static void OpenWindow() => GetWindow<SkillCSVImporter>("Skill CSV Importer");

    protected override string WindowTitle => "스킬 CSV → SO 변환기";
    protected override string DefaultCSVPath => "Assets/Resources/Data/CSV/skills.csv";
    protected override string DefaultOutputPath => "Assets/Resources/Data/SO/Skills";
    protected override int RequiredColumnCount => 6;

    // CSV 컬럼 순서:
    // 0: skillName    1: skillId      2: isUnlocked   3: damage
    // 4: cooldown     5: description

    protected override void ParseAndCreateSO(string[] columns, int lineIndex)
    {
        string parsedName = columns[0].Trim();

        SaveOrUpdateAsset<SkillData>(parsedName, so =>
        {
            so.skillName = parsedName;
            so.skillId = ParseInt(columns[1]);
            bool.TryParse(columns[2].Trim(), out so.isUnlocked);
            so.damage = ParseInt(columns[3]);
            so.cooldown = ParseFloat(columns[4]);
            so.description = columns[5].Trim();

            // so.icon은 안 건드림 → 인스펙터에서 등록
        });
    }
}