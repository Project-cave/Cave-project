using UnityEngine;

[CreateAssetMenu(fileName = "Skill",menuName ="Scriptable Object/skill")]
public class SkillSo : ScriptableObject
{
    public string skillName;
    public int damage;
    public int coolTime;
    public int cost;
    public int Range;

    [TextArea]
    public string Desc;
}
