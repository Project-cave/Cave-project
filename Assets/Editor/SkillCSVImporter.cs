using UnityEditor;
using UnityEngine;

public class SkillCSVImporter : CSVImporterBase
{
    [MenuItem("Tools/CSV Importer/Skill Importer")]
    public static void OpenWindow() => GetWindow<SkillCSVImporter>("Skill CSV Importer");

    protected override string WindowTitle       => "스킬 CSV → SO 변환기";
    protected override string DefaultCSVPath    => "Assets/Resources/Data/CSV/skills.csv";
    protected override string DefaultOutputPath => "Assets/Resources/Data/SO/Skills";
    protected override int RequiredColumnCount  => 7;

    protected override void ParseAndCreateSO(string[] columns, int lineIndex)
    {
        SkillData skill = CreateInstance<SkillData>();
        skill.skillName = columns[0].Trim();
        skill.skillId = ParseInt(columns[1]);
        bool.TryParse(columns[2].Trim(), out skill.isUnlocked);
        skill.damage = ParseInt(columns[3]);
        skill.cooldown = ParseFloat(columns[4]);
        skill.icon = Resources.Load<Sprite>($"Icons/{columns[5]}");
        skill.description = columns[6].Trim();

        SaveAsset(skill, skill.skillName);
    }
}
